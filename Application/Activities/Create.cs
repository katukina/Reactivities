using Domain;
using FluentValidation;
using MediatR;
using Persistence;
using Application.Core;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
    public class Create
    {
        //query return data, command do not (difference with List and Details classes)
        public class Commnad : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Commnad>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }
        public class Handler : IRequestHandler<Commnad, Result<Unit>>
        {                
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;                
            }
            public async Task<Result<Unit>> Handle(Commnad request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => 
                    x.UserName == _userAccessor.GetUsername());

                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = request.Activity,
                    IsHost = true
                };

                request.Activity.Attendees.Add(attendee);    
                            
                _context.Activities.Add(request.Activity); // Adding the activty in memory
                var result = await _context.SaveChangesAsync() > 0;

                 if (!result) return Result<Unit>.Failure("Failed to create activity");
                 return Result<Unit>.Success(Unit.Value); //Leting API controller in our case ActivitiesController that this has finished
            }
        }        
    }
}
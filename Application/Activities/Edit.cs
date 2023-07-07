using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Commnad : IRequest
        {   
            public Activity Activity { get; set; }
        }
        public class Handler : IRequestHandler<Commnad>
        {                
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Unit> Handle(Commnad request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                //Updating a single property can be activity.Title = request.Activity.Title ?? activity.Title; //but we need all the object
                _mapper.Map(request.Activity, activity);              
                await _context.SaveChangesAsync();
                return Unit.Value; //Leting API controller in our case ActivitiesController that this has finished
            }
        }
    }
}
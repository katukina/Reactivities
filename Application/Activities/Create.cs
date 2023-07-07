

using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        //query return data, command do not (difference with List and Details classes)
        public class Commnad : IRequest
        {
            public Activity Activity { get; set; }

            public class Handler : IRequestHandler<Commnad>
            {                
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;                
            }
                public async Task<Unit> Handle(Commnad request, CancellationToken cancellationToken)
                {
                    _context.Activities.Add(request.Activity); // Adding the activty in memory
                    await _context.SaveChangesAsync();
                    return Unit.Value; //Leting API controller in our case ActivitiesController that this has finished
                }
            }
        }
    }
}
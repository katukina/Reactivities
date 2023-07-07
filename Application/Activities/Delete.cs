using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        public class Commnad : IRequest
        {
            public Guid Id { get; set; }

            public class Handler : IRequestHandler<Commnad>
            {                
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;                
            }
                public async Task<Unit> Handle(Commnad request, CancellationToken cancellationToken)
                {
                    var activity = await _context.Activities.FindAsync(request.Id);
                    _context.Remove(activity);
                    await _context.SaveChangesAsync();
                    return Unit.Value; //Leting API controller in our case ActivitiesController that this has finished
                }
            }
        }
    }    
}
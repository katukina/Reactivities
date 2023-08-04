using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.ActivityAttendees
                    .Where(x => x.AppUser.UserName == request.Username)
                    .OrderBy(d => d.Activity.Date)
                    .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider) // To get from ActivityAttendees to UserActivityDto needs to add a mapper Application\Core\MappingProfiles.cs
                    .AsQueryable();

                var today = DateTime.Now.Date;

                //Using a predicate past query the activities in past
                if (request.Predicate == "past") 
                {
                    query = query.Where(x => x.Date <= today);
                }
                //Using a predicate hosting query the activities the user is hosting
                else if (request.Predicate == "hosting")
                {
                    query = query.Where(x => x.HostUsername == request.Username);
                }
                else {
                    query = query.Where(x => x.Date >= today );
                }

                /*Made by instructor
                query = request.Predicate switch
                {
                    "past" => query.Where(a => a.Date <= today),
                    "hosting" => query.Where(a => a.HostUsername == request.Username),
                    _ => query.Where(a => a.Date >= today)
                };*/

                var activities = await query.ToListAsync();

                return Result<List<UserActivityDto>>.Success(activities);
            }
        }
    }
}
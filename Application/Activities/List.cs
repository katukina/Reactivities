//Query handler to retrieve list of activities
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        //public class Query : IRequest<Result<List<ActivityDto>>>{}  //From interfaces mediator installed
        public class Query : IRequest<Result<PagedList<ActivityDto>>> //To Uses pagination PagedList class created in Core
        {
            public ActivityParams Params { get; set; } // created also in Core
        }


        // public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>> //To Uses pagination
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                 _mapper = mapper;
                _context = context;                
            }
            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                /*var activities = await _context.Activities
                //Could be added here hand code by using Select function from System.Linq and do it manually but mapper should be easy
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new{currentUsername = _userAccessor.GetUsername()})
                    .ToListAsync();

                return Result<PagedList<ActivityDto>>.Success(activities);*/
                var query = _context.Activities // await removed as we are not executing anything in the DB
                    .Where(x => x.Date >= request.Params.StartDate)
                    .OrderBy(d => d.Date)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .AsQueryable();

                //modify the query depending of request parameters (filtering)
                if (request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                if (request.Params.IsHost && !request.Params.IsGoing) //just the activity the user is hosting
                {
                    //because of projection we're working with actitvityDto which mean we have access to host user name inside here
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }
                //Call the method create in PagedList in Core
                return Result<PagedList<ActivityDto>>
                    .Success(await PagedList<ActivityDto>.CreateAsync(query,
                        request.Params.PageNumber, request.Params.PageSize));
            }
        }
    }
}
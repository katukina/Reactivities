using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using static Domain.UserFollowings;

namespace Application.Followers
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            //target user that this user is attempting to follow
            //this user (observer username) is gotten from the the token that is using to authenticate
            public string TargetUsername { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //the user to follow the target user
                var observer = await _context.Users.FirstOrDefaultAsync(x =>
                    x.UserName == _userAccessor.GetUsername());
                  
                //we assume that they are authenticated by the time they get to the stage that we have a user object inside the observer
                var target = await _context.Users.FirstOrDefaultAsync(x =>
                    x.UserName == request.TargetUsername);

                if (target == null) return null;

                // get the following from the database
                var following = await _context.UserFollowings.FindAsync(observer.Id, target.Id);

                //In Client is goint ot be a button follow/unfollow if not exits in DB add else remove
                if (following == null)
                {
                    following = new UserFollowing
                    {
                        Observer = observer,
                        Target = target
                    };

                    _context.UserFollowings.Add(following);
                }
                else
                {
                    _context.UserFollowings.Remove(following);
                }

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Failed to update following");
            }
        }

    }
}
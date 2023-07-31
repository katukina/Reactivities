using Microsoft.AspNetCore.Identity;
using static Domain.UserFollowings;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        //user can have many activities
        public ICollection<ActivityAttendee> Activities { get; set; }
        //user can have many photos
        public ICollection<Photo> Photos { get; set; }
        //Current user following
        public ICollection<UserFollowing> Followings { get; set; }
        //Who is following the current logged user
        public ICollection<UserFollowing> Followers { get; set; }
    }
}
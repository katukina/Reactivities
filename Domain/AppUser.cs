using Microsoft.AspNetCore.Identity;

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
    }
}
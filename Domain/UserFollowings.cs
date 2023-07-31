namespace Domain
{
    public class UserFollowings
    {
    public class UserFollowing
    {
        //The person connected (logged in)
        public string ObserverId { get; set; }
        public AppUser Observer { get; set; }
        //target for that person
        public string TargetId { get; set; }
        public AppUser Target { get; set; }
    }        
    }
}
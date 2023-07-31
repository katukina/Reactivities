namespace Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public AppUser Author { get; set; }
        public Activity Activity { get; set; }
        //Initial vallue set, meaning the date time is going to be stored in our database
        //no matter where the comment is beigin added from anywhere in the world is stored in DB at UTC time
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;        
    }
}
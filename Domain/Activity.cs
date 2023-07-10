namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; } //primary key of DB table, should be called Id in order migration can use it
        public string Title { get; set; } //title of the activity
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
    }
}
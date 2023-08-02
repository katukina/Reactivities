using System.Text.Json.Serialization;

namespace Application.Profiles
{
    public class UserActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        //a property that we add to help but not actually need to return to the client we can add attribute called Jsonignore
        [JsonIgnore]
        public string HostUsername { get; set; }
    }
}
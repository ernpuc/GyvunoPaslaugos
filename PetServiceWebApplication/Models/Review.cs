using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; }


        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Service? Service { get; set; }
    }
}

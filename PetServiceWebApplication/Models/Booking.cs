using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }


        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Service? Service { get; set; }
        [JsonIgnore]
        public ICollection<Payment>? Payments { get; set; }
    }
}

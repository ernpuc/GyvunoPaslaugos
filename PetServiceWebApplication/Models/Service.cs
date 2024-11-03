using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }


        [JsonIgnore]
        public PetServiceProvider? PetServiceProvider { get; set; }
        [JsonIgnore]
        public ICollection<Booking>? Bookings { get; set; }
        [JsonIgnore]
        public ICollection<Review>? Reviews { get; set; }
    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; } 
        [JsonIgnore] 
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [JsonIgnore] 
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

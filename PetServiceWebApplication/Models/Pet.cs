using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; }
        public string? Breed { get; set; }
        public string? Size { get; set; }
        public DateTime? BirthDate { get; set; }


        [JsonIgnore]
        public User? User { get; set; }
    }
}

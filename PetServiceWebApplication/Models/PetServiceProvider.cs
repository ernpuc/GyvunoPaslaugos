using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class PetServiceProvider
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }


        [JsonIgnore]
        public ICollection<Service>? Services { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Name cannot exceed 15 characters.")]
        public required string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }

        [JsonIgnore]
        public ICollection<PetServiceProvider> ManagedProviders { get; set; } = new List<PetServiceProvider>();
    }
}

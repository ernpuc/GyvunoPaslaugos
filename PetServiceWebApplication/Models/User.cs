using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "First name cannot exceed 15 characters.")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Last name cannot exceed 20 characters.")]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

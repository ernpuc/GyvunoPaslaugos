using Microsoft.AspNetCore.Identity;
using PetServiceWebApplication.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(15, ErrorMessage = "First name cannot exceed 15 characters.")]
    public required string FirstName { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "Last name cannot exceed 20 characters.")]
    public required string LastName { get; set; }

    [JsonIgnore]
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [JsonIgnore]
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

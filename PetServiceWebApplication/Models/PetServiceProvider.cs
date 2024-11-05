using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class PetServiceProvider
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public required string Address { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public required string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }

        [Required]
        public required ProviderCategory Category { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid image URL format.")]
        public string? Image { get; set; }

        [Required]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "Opening time must be a valid time of day.")]
        public TimeSpan OpeningTime { get; set; } = new TimeSpan(9, 0, 0);

        [Required]
        [Range(typeof(TimeSpan), "00:00:00", "23:59:59", ErrorMessage = "Closing time must be a valid time of day.")]
        [Compare(nameof(OpeningTime), ErrorMessage = "Closing time must be later than opening time.")]
        public TimeSpan ClosingTime { get; set; } = new TimeSpan(17, 0, 0);

        public List<DayOfWeek> AvailableDays { get; set; } = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        [JsonIgnore]
        public ICollection<Service> Services { get; set; } = new List<Service>();

        public double Rating => Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0.0;

        public int AdminId { get; set; }

        [JsonIgnore]
        public required Admin Admin { get; set; }

        public enum ProviderCategory
        {
            Grooming,
            Training,
            Boarding,
            Veterinary
        }
    }
}

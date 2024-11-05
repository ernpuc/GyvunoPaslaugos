using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class PetServiceProvider
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required ProviderCategory Category { get; set; }

        public string? Description { get; set; }
        public string? Image { get; set; }

        public TimeSpan OpeningTime { get; set; } = new TimeSpan(9, 0, 0);
        public TimeSpan ClosingTime { get; set; } = new TimeSpan(17, 0, 0);
        public List<DayOfWeek> AvailableDays { get; set; } = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        [JsonIgnore]
        public ICollection<Service> Services { get; set; } = new List<Service>();

        public double Rating => Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0.0;
    }

    public enum ProviderCategory
    {
        Grooming,
        Training,
        Boarding,
        Veterinary
    }
}

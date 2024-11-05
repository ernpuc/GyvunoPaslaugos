using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters.")]
        public required string Name { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Service description cannot exceed 1000 characters.")]
        public required string Description { get; set; }

        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public required decimal Price { get; set; }

        [Required]
        [Range(typeof(TimeSpan), "00:05:00", "24:00:00", ErrorMessage = "Service duration must be between 5 minutes and 24 hours.")]
        public TimeSpan Duration { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Service type cannot exceed 50 characters.")]

        public required ServiceType ServiceType { get; set; }

        public int PetServiceProviderId { get; set; }

        [JsonIgnore]
        public required PetServiceProvider PetServiceProvider { get; set; }

        public bool IsActive { get; set; } = true;

        [Url(ErrorMessage = "Invalid image URL format.")]
        public string? Image { get; set; }

        [Required]
        public required AnimalType TargetAnimal { get; set; }

    }

    public enum ServiceType
    {
        BasicGrooming,
        AdvancedGrooming,
        DeluxeGrooming,

        BasicTraining,
        ObedienceTraining,
        AdvancedTraining,

        ShortTermBoarding,
        LongTermBoarding,
        SpecialCareBoarding,

        GeneralCheckup,
        Vaccination,
        Surgery
    }

    public enum AnimalType
    {
        Dog,
        Cat,
        Bird,
        Reptile,
        Other
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Service
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public required ServiceType ServiceType { get; set; }

        public int PetServiceProviderId { get; set; }

        [JsonIgnore]
        public required PetServiceProvider PetServiceProvider { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Image { get; set; }

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

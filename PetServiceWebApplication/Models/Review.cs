using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5, ErrorMessage = "Please provide a rating between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public required DateTime Date { get; set; }

        public int PetServiceProviderId { get; set; }
        [JsonIgnore]
        public required PetServiceProvider PetServiceProvider { get; set; }

        public required string ApplicationUserId { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }
    }
}

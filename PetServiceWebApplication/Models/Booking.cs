using System;
using System.ComponentModel.DataAnnotations;

namespace PetServiceWebApplication.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public required string ApplicationUserId { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }

        public int ServiceId { get; set; }
        public required Service Service { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime RequestedServiceDate { get; set; }

        [Required]
        public TimeSpan RequestedServiceTime { get; set; }

        public DateTime RequestedServiceEndTime => RequestedServiceDate.Add(RequestedServiceTime).Add(Service.Duration);

        public bool IsCompleted { get; set; } = false;
        public bool IsPaid { get; set; } = false;
    }
}

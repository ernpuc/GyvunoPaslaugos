using System.Text.Json.Serialization;

namespace PetServiceWebApplication.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string? PaymentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime InitiationTs { get; set; }
        // public string? StripePayId { get; set; }
        public DateTime? ActualPayTs { get; set; }
        public string? Status { get; set; }


        [JsonIgnore]
        public Booking? Booking { get; set; }
    }
}

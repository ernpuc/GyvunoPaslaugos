namespace PetServiceWebApplication.Models
{
    public class BookingViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal ServicePrice { get; set; }
        public DateTime RequestedServiceDate { get; set; }
        public TimeSpan RequestedServiceTime { get; set; }
    }
}

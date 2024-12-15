namespace PetServiceWebApplication.Models
{
    public class ProviderInfoDTO
    {
        public PetServiceProvider Provider { get; set; }
        public List<Service> Services { get; set; }
    }
}
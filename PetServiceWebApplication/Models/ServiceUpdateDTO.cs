using System.ComponentModel;

namespace PetServiceWebApplication.Models
{
    public class ServiceUpdateDTO
    {
        public Service Service { get; set; }
        public PetServiceProvider.ProviderCategory Category { get; set; }
    }
}
using Microsoft.AspNetCore.Http;

namespace PetServiceWebApplication.Models
{
    public class ProviderImageDTO
    {
        public PetServiceProvider Provider { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
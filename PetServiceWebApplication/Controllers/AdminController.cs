using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;
using System.Linq;
using System.Security.Claims;

namespace PetServiceWebApplication.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in claims.");
        }

        [HttpPost("provider/add")]
        public IActionResult AddProvider([FromBody] PetServiceProvider provider)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            provider.ApplicationUserId = GetCurrentUserId();
            _context.PetServiceProviders.Add(provider);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProviderById), new { id = provider.Id }, provider);
        }

        [HttpGet("provider/{id}")]
        public IActionResult GetProviderById(int id)
        {
            var provider = _context.PetServiceProviders.Find(id);
            if (provider == null || provider.ApplicationUserId != GetCurrentUserId())
                return NotFound();
            return Ok(provider);
        }

        [HttpPut("provider/update/{id}")]
        public IActionResult UpdateProvider(int id, [FromBody] PetServiceProvider provider)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProvider = _context.PetServiceProviders.Find(id);
            if (existingProvider == null || existingProvider.ApplicationUserId != GetCurrentUserId())
                return NotFound();

            existingProvider.Name = provider.Name;
            existingProvider.Address = provider.Address;
            existingProvider.Phone = provider.Phone;
            existingProvider.Email = provider.Email;
            existingProvider.Description = provider.Description;
            existingProvider.OpeningTime = provider.OpeningTime;
            existingProvider.ClosingTime = provider.ClosingTime;
            existingProvider.AvailableDays = provider.AvailableDays;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("provider/delete/{id}")]
        public IActionResult DeleteProvider(int id)
        {
            var provider = _context.PetServiceProviders.Find(id);
            if (provider == null || provider.ApplicationUserId != GetCurrentUserId())
                return NotFound();

            _context.PetServiceProviders.Remove(provider);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPost("service/add")]
        public IActionResult AddService([FromBody] Service service)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var provider = _context.PetServiceProviders.Find(service.PetServiceProviderId);
            if (provider == null || provider.ApplicationUserId != GetCurrentUserId())
                return NotFound();

            _context.Services.Add(service);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
        }

        [HttpGet("service/{id}")]
        public IActionResult GetServiceById(int id)
        {
            var service = _context.Services
                .Include(s => s.PetServiceProvider)
                .FirstOrDefault(s => s.Id == id);
            if (service == null || service.PetServiceProvider.ApplicationUserId != GetCurrentUserId())
                return NotFound();
            return Ok(service);
        }

        [HttpPut("service/update/{id}")]
        public IActionResult UpdateService(int id, [FromBody] Service service)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingService = _context.Services
                .Include(s => s.PetServiceProvider)
                .FirstOrDefault(s => s.Id == id);
            if (existingService == null || existingService.PetServiceProvider.ApplicationUserId != GetCurrentUserId())
                return NotFound();

            existingService.Name = service.Name;
            existingService.Description = service.Description;
            existingService.Price = service.Price;
            existingService.Duration = service.Duration;
            existingService.ServiceType = service.ServiceType;
            existingService.TargetAnimal = service.TargetAnimal;
            existingService.IsActive = service.IsActive;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("service/delete/{id}")]
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services
                .Include(s => s.PetServiceProvider)
                .FirstOrDefault(s => s.Id == id);
            if (service == null || service.PetServiceProvider.ApplicationUserId != GetCurrentUserId())
                return NotFound();

            _context.Services.Remove(service);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("bookings/provider/{providerId}")]
        public IActionResult GetBookingsByProvider(int providerId)
        {
            var provider = _context.PetServiceProviders
                .Include(p => p.Services)
                .ThenInclude(s => s.Bookings)
                .FirstOrDefault(p => p.Id == providerId);
            if (provider == null || provider.ApplicationUserId != GetCurrentUserId())
                return NotFound();

            var bookings = provider.Services.SelectMany(s => s.Bookings).ToList();
            return Ok(bookings);
        }

        [HttpPut("booking/cancel/{bookingId}")]
        public IActionResult CancelBooking(int bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.Service)
                .ThenInclude(s => s.PetServiceProvider)
                .FirstOrDefault(b => b.Id == bookingId);
            if (booking == null || booking.Service.PetServiceProvider.ApplicationUserId != GetCurrentUserId())
                return NotFound();

            booking.IsCompleted = false;
            _context.SaveChanges();
            return NoContent();
        }
    }
}

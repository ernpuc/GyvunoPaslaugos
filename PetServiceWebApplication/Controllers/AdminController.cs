using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;
using System.Drawing;
using System.Linq;
using System.Security.Claims;

namespace PetServiceWebApplication.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "ServiceAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ServiceProviders()
        {
            return View();
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in claims.");
        }

        [HttpPost("provider/add")]
        public IActionResult AddProvider([FromForm] ProviderImageDTO dto)
        {
            string imagePath = null;

            // Handle File Upload
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(dto.ImageFile.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProviderImages", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    dto.ImageFile.CopyTo(stream);
                }

                imagePath = "/ProviderImages/" + fileName;
            }
            // Handle Image URL
            else if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                imagePath = dto.ImageUrl;
            }
            
            dto.Provider.Image = imagePath;
            dto.Provider.ApplicationUserId = GetCurrentUserId();
            _context.PetServiceProviders.Add(dto.Provider);
            _context.SaveChanges();
            return Ok(new { success = true, message = "Provider added successfully." });
        }

        [HttpGet("provider/{id}")]
        public IActionResult GetProviderById(int id)
        {
            var provider = _context.PetServiceProviders.Find(id);
            if (provider == null || provider.ApplicationUserId != GetCurrentUserId())
                return NotFound();
            return Ok(provider);
        }

        [HttpGet("providers")]
        public IActionResult GetProvidersForAdmin()
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
                return Unauthorized();

            var providers = _context.PetServiceProviders
                                    .Where(p => p.ApplicationUserId == currentUserId)
                                    .ToList();

            if (!providers.Any())
                return NotFound();

            return Ok(providers);
        }

        [HttpPut("provider/update/{id}")]
        public IActionResult UpdateProvider(int id, [FromBody] PetServiceProvider provider)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { Errors = errors });
            }

            provider.ApplicationUserId = GetCurrentUserId();

            _context.Entry(provider).State = EntityState.Modified;

            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PetServiceProviders.Any(p => p.Id == id))
                    return NotFound($"Provider with ID {id} not found.");
                throw;
            }


            return NoContent();
        }

        [HttpDelete("provider/delete/{id}")]
        public IActionResult DeleteProvider(int id)
        {
            var provider = _context.PetServiceProviders
                                   .Include(p => p.Services)
                                       .ThenInclude(s => s.Bookings) 
                                   .Include(p => p.Reviews)
                                   .FirstOrDefault(p => p.Id == id);

            if (provider == null)
            {
                return NotFound(new { message = "Provider not found." });
            }

            // Delete Bookings for each Service
            foreach (var service in provider.Services)
            {
                if (service.Bookings != null && service.Bookings.Any())
                {
                    _context.Bookings.RemoveRange(service.Bookings);
                }
            }

            // Delete related Services
            if (provider.Services != null && provider.Services.Any())
            {
                _context.Services.RemoveRange(provider.Services);
            }

            // Delete related Reviews
            if (provider.Reviews != null && provider.Reviews.Any())
            {
                _context.Reviews.RemoveRange(provider.Reviews);
            }

            _context.PetServiceProviders.Remove(provider);

            _context.SaveChanges();

            return Ok(new { message = "Provider and all related entities deleted successfully." });
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

            //var existingService = _context.Services
            //    .Include(s => s.PetServiceProvider)
            //    .FirstOrDefault(s => s.Id == id);
            //if (existingService == null || existingService.PetServiceProvider.ApplicationUserId != GetCurrentUserId())
            //    return NotFound();

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PetServiceProviders.Any(p => p.Id == id))
                    return NotFound($"Provider with ID {id} not found.");
                throw;
            }


            return NoContent();
        }

        [HttpDelete("service/delete/{id}")]
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services
                                  .Include(s => s.Bookings)
                                  .FirstOrDefault(s => s.Id == id);

            if (service == null)
            {
                return NotFound(new { message = "Service not found." });
            }

            if (service.Bookings != null && service.Bookings.Any())
            {
                _context.Bookings.RemoveRange(service.Bookings);
            }

            _context.Services.Remove(service);
            _context.SaveChanges();

            return Ok(new { message = "Service and associated bookings deleted successfully." });
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

        [HttpGet("ManageProvider/{id}")]
        public async Task<IActionResult> GetProviderWithServices(int id)
        {
            var provider = await _context.PetServiceProviders
                .FirstOrDefaultAsync(c => c.Id == id);

            if (provider == null)
                return NotFound($"No provider found with ID {id}.");

            var services = await _context.Services
                .Where(s => s.PetServiceProviderId == id)
                .ToListAsync();

            var model = new ProviderInfoDTO
            {
                Provider = provider,
                Services = services,
            };

            return View("ManageProvider", model);
        }

        [HttpGet("ManageProvider/UpdateService/{id}")]
        public async Task<IActionResult> GetServiceWithProviderCategory(int id)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound($"No service found with ID {id}.");

            var provider = await _context.PetServiceProviders
                .FirstAsync(p => p.Id == service.PetServiceProviderId);

            var model = new ServiceUpdateDTO
            {
                Service = service,
                Category = provider.Category,
            };

            return View("UpdateService", model);
        }
    }
}

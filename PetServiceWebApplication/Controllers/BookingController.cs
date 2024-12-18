using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;
using System.Security.Claims;

namespace PetServiceWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager;
    }

    public async Task<IActionResult> List()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized("You must be logged in to view your bookings.");
        }

        var bookings = await _context.Bookings
            .Where(b => b.ApplicationUserId == user.Id)
            .Include(b => b.Service)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        return View(bookings);
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found in claims.");
    }

    [HttpGet("book/service/{serviceId}")]
    public async Task<IActionResult> GetBookingForm(int serviceId)
    {
        var service = await _context.Services
            .Where(s => s.IsActive && s.Id == serviceId)
            .FirstOrDefaultAsync();

        if (service == null)
        {
            return NotFound("The selected service does not exist.");
        }

        var viewModel = new BookingViewModel
        {
            ServiceId = service.Id,
            ServiceName = service.Name,
            ServicePrice = service.Price
        };

        return View("BookingForm", viewModel);
    }


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] BookingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid input data.");
        }

        var serviceId = model.ServiceId;
        var user = await _userManager.GetUserAsync(User);
        var service = await _context.Services.FindAsync(serviceId);

        if (service == null)
        {
            return NotFound(new { message = "Service not found." });
        }

        var booking = new Booking
        {
            ServiceId = serviceId,
            Service = service,
            ApplicationUserId = user.Id,
            ApplicationUser = user,
            RequestedServiceDate = model.RequestedServiceDate,
            RequestedServiceTime = model.RequestedServiceTime,
            BookingDate = DateTime.Now
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Booking confirmed successfully." });
    }


    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetBookingById([FromRoute] int id)
    //{
    //    var booking = await _context.Bookings.FindAsync(id);

    //    if (booking == null)
    //        return NotFound($"Booking with ID {id} not found.");

    //    return Ok(booking);
    //}


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking([FromRoute] int id, [FromBody] Booking booking)
    {
        if (id != booking.Id)
            return BadRequest("Booking ID mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(booking).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Bookings.Any(b => b.Id == id))
                return NotFound($"Booking with ID {id} not found.");
            throw;
        }

        return NoContent();
    }

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> CancelBooking([FromRoute] int id)
    //{
    //    Booking? booking = await _context.Bookings.FindAsync(id);

    //    if (booking == null)
    //        return NotFound($"Booking with ID {id} not found.");

    //    _context.Bookings.Remove(booking);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}
}

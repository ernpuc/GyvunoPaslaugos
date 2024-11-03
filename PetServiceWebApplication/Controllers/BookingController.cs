using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById([FromRoute] int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound($"Booking with ID {id} not found.");

        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBookingsByUser([FromRoute] int userId)
    {
        User? user = await _context.Users.FindAsync(userId);

        if (user == null)
            return BadRequest($"User with ID {userId} not found.");

        List<Booking> bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .ToListAsync();

        if (bookings.Count == 0)
            return NotFound("No bookings found for this user.");

        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
    }

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking([FromRoute] int id)
    {
        Booking? booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound($"Booking with ID {id} not found.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

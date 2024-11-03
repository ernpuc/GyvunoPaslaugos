using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;

    public PaymentController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById([FromRoute] int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
            return NotFound($"Payment with ID {id} not found.");

        return Ok(payment);
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetPaymentsByBooking(int bookingId)
    {
        var payments = await _context.Payments.Where(p => p.BookingId == bookingId).ToListAsync();

        if (payments == null || payments.Count == 0)
            return NotFound($"No payments found for booking ID {bookingId}.");

        return Ok(payments);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPaymentsByUser(int userId)
    {
        var payments = await
        (
            from payment in _context.Payments
            join booking in _context.Bookings
                on payment.BookingId equals booking.Id
            where booking.UserId == userId
            select payment
        ).ToListAsync();


        if (payments == null || payments.Count == 0)
            return NotFound($"No payments found for user ID {userId}.");

        return Ok(payments);
    }

    [HttpGet("clinic/{ProviderId}")]
    public async Task<IActionResult> GetPaymentsByClinic(int ProviderId)
    {
        var payments = await
        (
            from payment in _context.Payments
            join booking in _context.Bookings
                on payment.BookingId equals booking.Id
            join service in _context.Services
                on booking.ServiceId equals service.Id
            where service.ProviderId == ProviderId
            select payment
        ).ToListAsync();


        if (payments == null || payments.Count == 0)
            return NotFound($"No payments found for clinic ID {ProviderId}.");

        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var bookingExists = await _context.Bookings.AnyAsync(b => b.Id == payment.BookingId);
        if (!bookingExists)
            return NotFound($"Booking with ID {payment.BookingId} does not exist.");

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] string newStatus)
    {
        if (string.IsNullOrEmpty(newStatus))
            return BadRequest("Payment status cannot be empty.");

        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            return NotFound($"Payment with ID {id} does not exist.");

        payment.Status = newStatus;
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById([FromRoute] int id)
    {
        var review = await _context.Reviews.FindAsync(id);

        if (review == null)
            return NotFound($"Review with ID {id} not found.");

        return Ok(review);
    }

    [HttpGet("clinic/{ProviderId}")]
    public async Task<IActionResult> GetReviewsByClinic([FromRoute] int ProviderId)
    {
        var clinic = await _context.PetServiceProviders.FindAsync(ProviderId);
        if (clinic == null)
            return NotFound($"Clinic with ID {ProviderId} not found.");

        var reviews = await
        (
            from review in _context.Reviews
            join petServiceProvider in _context.PetServiceProviders
                on review.PetServiceProviderId equals petServiceProvider.Id
            where petServiceProvider.Id == ProviderId
            select review
        ).ToListAsync();

        if (reviews.Count == 0)
            return NotFound($"No reviews found for clinic with ID {ProviderId}.");

        return Ok(reviews);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetReviewsByUser([FromRoute] string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound($"User with ID {userId} not found.");

        var reviews = await _context.Reviews
            .Where(r => r.ApplicationUserId == userId)
            .ToListAsync();

        if (reviews.Count == 0)
            return NotFound($"No reviews found for user with ID {userId}.");

        return Ok(reviews);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateReview([FromBody] Review review)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var provider = await _context.PetServiceProviders.FindAsync(review.PetServiceProviderId);

        if (userId == null)
            return Unauthorized("User must be logged in.");
        if (provider == null)
            return NotFound($"Provider with ID {review.PetServiceProviderId} not found.");

        review.ApplicationUserId = userId;
        review.Date = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] Review review)
    //{
    //    if (id != review.Id || !ModelState.IsValid)
    //        return BadRequest();

    //    var existingReview = await _context.Reviews.FindAsync(id);
    //    if (existingReview == null)
    //        return NotFound($"Review with ID {id} not found.");

    //    _context.Entry(review).State = EntityState.Modified;

    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException)
    //    {
    //        if (!_context.Reviews.Any(r => r.Id == id))
    //            return NotFound($"Review with ID {id} not found.");
    //        throw;
    //    }

    //    return NoContent();
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteReview([FromRoute] int id)
    //{
    //    var review = await _context.Reviews.FindAsync(id);
    //    if (review == null)
    //        return NotFound($"Review with ID {id} not found.");

    //    _context.Reviews.Remove(review);
    //    await _context.SaveChangesAsync();

    //    return NoContent();
    //}
}

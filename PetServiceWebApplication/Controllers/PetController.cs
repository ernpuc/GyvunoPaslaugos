using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetController : Controller
{
    private readonly ApplicationDbContext _context;

    public PetController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPetsByUser([FromRoute] int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound($"User with ID {userId} not found.");

        var pets = await _context.Pets.Where(p => p.UserId == userId).ToListAsync();

        if (pets.Count == 0)
            return NotFound("No pets found for this user.");

        return Ok(pets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPetById([FromRoute] int id)
    {
        var pet = await _context.Pets.FindAsync(id);

        if (pet == null)
            return NotFound($"Pet with ID {id} not found.");

        return Ok(pet);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePet([FromBody] Pet pet)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPetById), new { id = pet.Id }, pet);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePet([FromRoute] int id, [FromBody] Pet pet)
    {
        if (id != pet.Id)
            return BadRequest("Pet ID mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(pet).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Pets.Any(p => p.Id == id))
                return NotFound($"Pet with ID {id} not found.");
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePet([FromRoute] int id)
    {
        var pet = await _context.Pets.FindAsync(id);

        if (pet == null)
            return NotFound($"Pet with ID {id} not found.");

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
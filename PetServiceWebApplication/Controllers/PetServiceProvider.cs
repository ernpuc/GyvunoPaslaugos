using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetServiceProviderController : Controller
{
    private readonly ApplicationDbContext _context;

    public PetServiceProviderController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClinics()
    {
        var clinics = await _context.PetServiceProviders.ToListAsync();

        if (clinics == null || clinics.Count == 0)
            return NotFound("No clinics found.");

        return Ok(clinics);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClinicById([FromRoute] int id)
    {
        var clinic = await _context.PetServiceProviders.FindAsync(id);

        if (clinic == null)
            return NotFound($"Clinic with ID {id} not found.");

        return Ok(clinic);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClinic([FromBody] PetServiceProvider clinic)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.PetServiceProviders.Add(clinic);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClinicById), new { id = clinic.Id }, clinic);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClinic([FromRoute] int id, [FromBody] PetServiceProvider clinic)
    {
        if (id != clinic.Id || !ModelState.IsValid)
            return BadRequest("Clinic ID mismatch or invalid data.");

        var existingClinic = await _context.PetServiceProviders.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (existingClinic == null)
            return NotFound($"Clinic with ID {id} not found.");

        _context.Entry(clinic).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.PetServiceProviders.Any(c => c.Id == id))
                return NotFound($"Clinic with ID {id} not found.");
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClinic([FromRoute] int id)
    {
        var clinic = await _context.PetServiceProviders.FindAsync(id);

        if (clinic == null)
            return NotFound($"Clinic with ID {id} not found.");

        _context.PetServiceProviders.Remove(clinic);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

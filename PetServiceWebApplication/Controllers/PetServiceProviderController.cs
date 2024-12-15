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

    [HttpGet("providers-by-criteria")]
    public async Task<IActionResult> SearchProvidersByCriteria(
        [FromQuery] string? location,
        [FromQuery] PetServiceProvider.ProviderCategory? category)
    {
        var providersQuery = _context.PetServiceProviders.AsQueryable();

        if (!string.IsNullOrWhiteSpace(location))
            providersQuery = providersQuery.Where(p => EF.Functions.Like(p.Address, $"%{location}%"));

        if (category.HasValue)
            providersQuery = providersQuery.Where(p => p.Category == category);

        var providers = await providersQuery.ToListAsync();

        return View("ProviderList", providers.Any() ? providers : new List<PetServiceProvider>());
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProviderById([FromRoute] int id)
    {
        var provider = await _context.PetServiceProviders.FindAsync(id);

        if (provider == null)
            return NotFound($"Provider with ID {id} not found.");

        return Ok(provider);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProvider([FromBody] PetServiceProvider provider)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.PetServiceProviders.Add(provider);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProviderById), new { id = provider.Id }, provider);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProvider([FromRoute] int id, [FromBody] PetServiceProvider provider)
    {
        if (id != provider.Id || !ModelState.IsValid)
            return BadRequest("Provider ID mismatch or invalid data.");

        var existingProvider = await _context.PetServiceProviders.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (existingProvider == null)
            return NotFound($"Provider with ID {id} not found.");

        _context.Entry(provider).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.PetServiceProviders.Any(c => c.Id == id))
                return NotFound($"Provider with ID {id} not found.");
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProvider([FromRoute] int id)
    {
        var provider = await _context.PetServiceProviders.FindAsync(id);

        if (provider == null)
            return NotFound($"Provider with ID {id} not found.");

        _context.PetServiceProviders.Remove(provider);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("provider/{id}/services")]
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

        return View("ProviderInfo", model);
    }

}

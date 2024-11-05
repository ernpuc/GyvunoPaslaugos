using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServiceWebApplication.Data;
using PetServiceWebApplication.Models;

namespace PetServiceWebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiceController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceById([FromRoute] int id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
            return NotFound($"Service with ID {id} not found.");

        return Ok(service);
    }

    [HttpGet("provider/{ProviderId}")]
    public async Task<IActionResult> GetServicesByProvider([FromRoute] int ProviderId)
    {
        var clinic = await _context.PetServiceProviders.FindAsync(ProviderId);
        if (clinic == null)
            return NotFound($"Service provider with ID {ProviderId} not found.");

        var services = await _context.Services
            .Where(s => s.PetServiceProviderId == ProviderId)
            .ToListAsync();

        if (services.Count == 0)
            return NotFound($"No services found for service provider with ID {ProviderId}.");

        return Ok(services);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] Service service)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var clinic = await _context.PetServiceProviders.FindAsync(service.PetServiceProviderId);
        if (clinic == null)
            return NotFound($"Service provider with ID {service.PetServiceProviderId} not found.");

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService([FromRoute] int id, [FromBody] Service service)
    {
        if (id != service.Id || !ModelState.IsValid)
            return BadRequest();

        var existingService = await _context.Services.FindAsync(id);
        if (existingService == null)
            return NotFound($"Service with ID {id} not found.");

        _context.Entry(service).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Services.Any(s => s.Id == id))
                return NotFound($"Service with ID {id} not found.");
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService([FromRoute] int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound($"Service with ID {id} not found.");

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

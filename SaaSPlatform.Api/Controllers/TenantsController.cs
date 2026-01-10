using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Api.Data;
using SaaSPlatform.Api.Models;
using SaaSPlatform.Api.Dtos;

namespace SaaSPlatform.Api.Controllers;

[ApiController]
[Route("api/tenants")]
public class TenantsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TenantsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required");

        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync();

        return Ok(tenant);
    }

    [HttpGet]
    public async Task<IActionResult> GetTenants()
    {
        var tenants = await _db.Tenants.ToListAsync();
        return Ok(tenants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenant = await _db.Tenants.FindAsync(id);
        if (tenant == null)
            return NotFound();

        return Ok(tenant);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantRequest request)
    {
        var tenant = await _db.Tenants.FindAsync(id);
        if (tenant == null)
            return NotFound();

        tenant.Name = request.Name;
        await _db.SaveChangesAsync();

        return Ok(tenant);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(Guid id)
    {
        var tenant = await _db.Tenants.FindAsync(id);
        if (tenant == null)
            return NotFound();

        _db.Tenants.Remove(tenant);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

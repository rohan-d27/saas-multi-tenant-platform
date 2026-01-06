using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Api.Data;
using SaaSPlatform.Api.Models;

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
    public async Task<IActionResult> CreateTenant(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Name is required");

        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            Name = name,
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
}

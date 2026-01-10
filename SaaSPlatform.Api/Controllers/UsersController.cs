using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Api.Common;
using SaaSPlatform.Api.Data;
using SaaSPlatform.Api.Models;

namespace SaaSPlatform.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TenantContext _tenantContext;

    public UsersController(AppDbContext db, TenantContext tenantContext)
    {
        _db = db;
        _tenantContext = tenantContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(string email)
    {
        var tenantId = _tenantContext.TenantId;
        // check tenant exists
        var tenantExists = await _db.Tenants.AnyAsync(t => t.TenantId == tenantId);
        if (!tenantExists)
            return BadRequest("Tenant does not exist");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = email,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var tenantId = _tenantContext.TenantId;
        var users = await _db.Users
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();

        return Ok(users);
    }
}

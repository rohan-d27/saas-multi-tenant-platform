using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Api.Data;
using SaaSPlatform.Api.Models;

namespace SaaSPlatform.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(string email, Guid tenantId)
    {
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
        return Ok(await _db.Users.ToListAsync());
    }
}

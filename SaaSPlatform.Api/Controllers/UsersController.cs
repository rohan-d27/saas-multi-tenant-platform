using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Api.Data;
using SaaSPlatform.Api.Models;
using SaaSPlatform.Api.Common;
using SaaSPlatform.Api.Dtos;

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
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var tenantId = _tenantContext.TenantId;
        
        var tenantExists = await _db.Tenants.AnyAsync(t => t.TenantId == tenantId);
        if (!tenantExists)
            return BadRequest("Tenant does not exist");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenantId = _tenantContext.TenantId;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.TenantId == tenantId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var tenantId = _tenantContext.TenantId;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.TenantId == tenantId);

        if (user == null)
            return NotFound();

        user.Email = request.Email;
        await _db.SaveChangesAsync();

        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var tenantId = _tenantContext.TenantId;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.TenantId == tenantId);

        if (user == null)
            return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

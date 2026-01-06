namespace SaaSPlatform.Api.Models;

public class User
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
}

using SaaSPlatform.Api.Common;

namespace SaaSPlatform.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        // Skip tenant validation for Swagger and other non-API paths
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (ShouldSkipTenantValidation(path))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantId))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("X-Tenant-Id header is missing");
            return;
        }

        if (!Guid.TryParse(tenantId, out var parsedTenantId))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Tenant Id");
            return;
        }

        tenantContext.TenantId = parsedTenantId;

        await _next(context);
    }

    private static bool ShouldSkipTenantValidation(string path)
    {
        return path.StartsWith("/swagger") ||
               path.StartsWith("/_vs/") ||
               path.StartsWith("/_framework/") ||
               path == "/" ||
               path == "/favicon.ico" ||
               path.StartsWith("/api/tenants"); // Allow tenant creation without header
    }
}

using Microsoft.EntityFrameworkCore;
using SaaSPlatform.Api.Common;
using SaaSPlatform.Api.Data;
using SaaSPlatform.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure database with error handling
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Warning: No database connection string found. Using in-memory database for development.");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("SaaSPlatform"));
}
else
{
    try
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        Console.WriteLine("PostgreSQL database configured successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"PostgreSQL configuration failed: {ex.Message}. Falling back to in-memory database.");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("SaaSPlatform"));
    }
}

// Add services to the container.
builder.Services.AddScoped<TenantContext>();
builder.Services.AddControllers();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add simple Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Try to ensure database is ready
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    if (context.Database.IsInMemory())
    {
        context.Database.EnsureCreated();
        app.Logger.LogInformation("In-memory database initialized successfully");
    }
    else
    {
        // For PostgreSQL, try to connect
        if (await context.Database.CanConnectAsync())
        {
            app.Logger.LogInformation("PostgreSQL database connection verified successfully");
        }
        else
        {
            app.Logger.LogWarning("PostgreSQL database connection failed, but continuing...");
        }
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database initialization failed: {Message}", ex.Message);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

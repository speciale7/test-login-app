using LoginApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoginApi.Tests;

/// <summary>
/// Custom WebApplicationFactory that replaces the SQLite DB with an in-memory database
/// to allow isolated, fast integration tests.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    // Store the DB name as a field so all requests within the same factory share the same DB.
    // Placing Guid.NewGuid() inside the AddDbContext lambda would regenerate it per scope
    // (i.e., per request), causing each request to use a separate, empty database.
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the real DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add an in-memory database using the fixed name for this factory instance
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }
}

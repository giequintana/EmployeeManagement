using System.Security.Cryptography;

using EmployeeManagement.Infrastructure.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.API.IntegrationTests.Infrastructure;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public const string AdminUsername = "test-admin";
    public const string AdminPassword = "Test-Admin-Password-1!";
    public const string UserUsername = "test-user";
    public const string UserPassword = "Test-User-Password-1!";

    private const string DefaultConnectionString =
        "Data Source=DESKTOP-E7RC706;Initial Catalog=EmployeeManagementDb_Tests;" +
        "Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Application Name=EmployeeManagement.Tests";

    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("EMS_TEST_CONNECTION_STRING") ?? DefaultConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
        builder.UseSetting("Jwt:SigningKey", Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));

        builder.UseSetting("SeedAdmin:Username", AdminUsername);
        builder.UseSetting("SeedAdmin:Email", "test-admin@example.com");
        builder.UseSetting("SeedAdmin:Password", AdminPassword);

        builder.UseSetting("SeedUser:Username", UserUsername);
        builder.UseSetting("SeedUser:Email", "test-user@example.com");
        builder.UseSetting("SeedUser:Password", UserPassword);
        builder.UseSetting("RateLimiting:LoginPermitLimit", "1000");
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var databaseName = context.Database.GetDbConnection().Database;

        if (!databaseName.EndsWith("_Tests", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Refusing to reset '{databaseName}'. Integration tests may only use a database whose name ends with '_Tests'.");
        }

        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
}
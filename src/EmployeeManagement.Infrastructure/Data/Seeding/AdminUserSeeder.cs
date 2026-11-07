using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Infrastructure.Data.Seeding;

internal static class AdminUserSeeder
{
    public static void Seed(DbContext context, IConfiguration configuration)
    {
        var settings = ReadSettings(configuration);
        if (settings is null)
        {
            return;
        }

        var users = context.Set<ApplicationUser>();
        if (users.Any(u => u.Username == settings.Username))
        {
            return;
        }

        users.Add(CreateAdmin(settings));
        context.SaveChanges();
    }

    public static async Task SeedAsync(
        DbContext context,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var settings = ReadSettings(configuration);
        if (settings is null)
        {
            return;
        }

        var users = context.Set<ApplicationUser>();
        if (await users.AnyAsync(u => u.Username == settings.Username, cancellationToken))
        {
            return;
        }

        users.Add(CreateAdmin(settings));
        await context.SaveChangesAsync(cancellationToken);
    }

    private static SeedAdminSettings? ReadSettings(IConfiguration configuration)
    {
        var section = configuration.GetSection("SeedAdmin");
        var username = section["Username"];
        var email = section["Email"];
        var password = section["Password"];

        if (string.IsNullOrWhiteSpace(username)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        return new SeedAdminSettings(username, email, password);
    }

    private static ApplicationUser CreateAdmin(SeedAdminSettings settings)
    {
        var admin = new ApplicationUser
        {
            Username = settings.Username,
            Email = settings.Email,
            Role = UserRole.Admin
        };

        admin.PasswordHash = new PasswordHasher<ApplicationUser>()
            .HashPassword(admin, settings.Password);

        return admin;
    }

    private sealed record SeedAdminSettings(string Username, string Email, string Password);
}
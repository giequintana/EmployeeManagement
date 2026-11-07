using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Infrastructure.Data.Seeding;

internal static class UserSeeder
{
    private static readonly (string SectionName, UserRole Role)[] SeedAccounts =
    [
        ("SeedAdmin", UserRole.Admin),
        ("SeedUser", UserRole.User)
    ];

    public static void Seed(DbContext context, IConfiguration configuration)
    {
        var users = context.Set<ApplicationUser>();
        var added = false;

        foreach (var account in ReadAccounts(configuration))
        {
            if (users.Any(u => u.Username == account.Username))
            {
                continue;
            }

            users.Add(CreateUser(account));
            added = true;
        }

        if (added)
        {
            context.SaveChanges();
        }
    }

    public static async Task SeedAsync(
        DbContext context,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var users = context.Set<ApplicationUser>();
        var added = false;

        foreach (var account in ReadAccounts(configuration))
        {
            if (await users.AnyAsync(u => u.Username == account.Username, cancellationToken))
            {
                continue;
            }

            users.Add(CreateUser(account));
            added = true;
        }

        if (added)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static IEnumerable<SeedAccount> ReadAccounts(IConfiguration configuration)
    {
        foreach (var (sectionName, role) in SeedAccounts)
        {
            var section = configuration.GetSection(sectionName);
            var username = section["Username"];
            var email = section["Email"];
            var password = section["Password"];

            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(password))
            {
                continue;
            }

            yield return new SeedAccount(username, email, password, role);
        }
    }

    private static ApplicationUser CreateUser(SeedAccount account)
    {
        var user = new ApplicationUser
        {
            Username = account.Username,
            Email = account.Email,
            Role = account.Role
        };

        user.PasswordHash = new PasswordHasher<ApplicationUser>()
            .HashPassword(user, account.Password);

        return user;
    }

    private sealed record SeedAccount(string Username, string Email, string Password, UserRole Role);
}
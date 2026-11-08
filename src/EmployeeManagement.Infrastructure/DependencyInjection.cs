using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Infrastructure.Authentication;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Data.Interceptors;
using EmployeeManagement.Infrastructure.Data.Seeding;
using EmployeeManagement.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            });

            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>());

            options.UseSeeding((context, _) =>
                AdminUserSeeder.Seed(context, configuration));

            options.UseAsyncSeeding((context, _, cancellationToken) =>
                AdminUserSeeder.SeedAsync(context, configuration, cancellationToken));

            options.UseSeeding((context, _) =>
                UserSeeder.Seed(context, configuration));

            options.UseAsyncSeeding((context, _, cancellationToken) =>
                UserSeeder.SeedAsync(context, configuration, cancellationToken));
        });

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDashboardQueries, DashboardQueries>();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt:Issuer is required.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience is required.")
            .Validate(o => o.AccessTokenLifetimeMinutes is > 0 and <= 1440,
                "Jwt:AccessTokenLifetimeMinutes must be between 1 and 1440.")
            .Validate(o => JwtOptions.IsValidSigningKey(o.SigningKey),
                "Jwt:SigningKey must be a Base64 string of at least 32 bytes. Set it with dotnet user-secrets.")
            .ValidateOnStart();

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

        return services;
    }
}
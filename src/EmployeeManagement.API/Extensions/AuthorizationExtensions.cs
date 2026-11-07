using EmployeeManagement.API.Authorization;
using EmployeeManagement.Core.Enums;

using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.API.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build())
            .AddPolicy(Policies.ManageEmployees, policy =>
                policy.RequireRole(nameof(UserRole.Admin)))
            .AddPolicy(Policies.ManageDepartments, policy =>
                policy.RequireRole(nameof(UserRole.Admin)));

        return services;
    }
}
namespace EmployeeManagement.API.Extensions;

public static class CorsExtensions
{
    public const string ClientPolicy = "Client";

    public static IServiceCollection AddClientCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (environment.IsProduction())
        {
            ValidateProductionOrigins(allowedOrigins);
        }

        services.AddCors(options => options.AddPolicy(ClientPolicy, policy =>
        {
            if (allowedOrigins.Length == 0)
            {
                return;
            }

            policy
                .WithOrigins(allowedOrigins)
                .WithMethods("GET", "POST", "PUT", "DELETE")
                .WithHeaders("Authorization", "Content-Type")
                .WithExposedHeaders("Location", "WWW-Authenticate");
        }));

        return services;
    }

    private static void ValidateProductionOrigins(string[] origins)
    {
        if (origins.Length == 0)
        {
            throw new InvalidOperationException(
                "Cors:AllowedOrigins must contain the frontend URL in Production.");
        }

        var invalid = origins
            .Where(origin =>
                origin.EndsWith('/')
                || !Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                || uri.Scheme != Uri.UriSchemeHttps)
            .ToArray();

        if (invalid.Length > 0)
        {
            throw new InvalidOperationException(
                $"Cors:AllowedOrigins must be HTTPS origins without a trailing slash. Invalid: {string.Join(", ", invalid)}");
        }
    }
}
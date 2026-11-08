namespace EmployeeManagement.API.Extensions;

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(
        this IApplicationBuilder app,
        IHostEnvironment environment)
    {
        var addContentSecurityPolicy = !environment.IsDevelopment();

        return app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;

                headers.XContentTypeOptions = "nosniff";
                headers.XFrameOptions = "DENY";
                headers["Referrer-Policy"] = "no-referrer";
                headers.CacheControl = "no-store";

                if (addContentSecurityPolicy)
                {
                    headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'";
                }

                return Task.CompletedTask;
            });

            await next();
        });
    }
}
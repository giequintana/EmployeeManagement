using System.Text.Json.Serialization;

using EmployeeManagement.API.Extensions;
using EmployeeManagement.API.Filters;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.API.OpenApi;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;

using Serilog;

const long MaxRequestBodyBytes = 1_048_576; // 1 MB: far above any JSON this API accepts

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs", "startup-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.AddServerHeader = false;
        options.Limits.MaxRequestBodySize = MaxRequestBodyBytes;
    });
    builder.Services.Configure<IISServerOptions>(options =>
        options.MaxRequestBodySize = MaxRequestBodyBytes);

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.File(
            Path.Combine(builder.Environment.ContentRootPath, "logs", "api-.log"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            fileSizeLimitBytes: 10 * 1024 * 1024,
            rollOnFileSizeLimit: true,
            shared: true,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {TraceId} {SourceContext}: {Message:lj}{NewLine}{Exception}"));

    builder.Services
        .AddControllers(options => options.Filters.Add<FluentValidationFilter>())
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<ConstraintViolationExceptionHandler>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddOpenApi(options =>
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddJwtAuthentication();
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddClientCors(builder.Configuration, builder.Environment);
    builder.Services.AddApiRateLimiting(builder.Configuration);

    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(180);
        options.IncludeSubDomains = false;
        options.Preload = false;
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestHost}{RequestPath} responded {StatusCode} in {Elapsed:0} ms for {UserName} from {ClientIp}";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserName", httpContext.User.Identity?.Name ?? "anonymous");
            diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        };
    });

    app.UseSecurityHeaders(app.Environment);
    app.UseExceptionHandler();
    app.UseStatusCodePages();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi().AllowAnonymous();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Employee Management API v1");
            options.RoutePrefix = "swagger";
            options.EnablePersistAuthorization();
        });
    }
    else
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseCors(CorsExtensions.ClientPolicy);
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "API failed to start");
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
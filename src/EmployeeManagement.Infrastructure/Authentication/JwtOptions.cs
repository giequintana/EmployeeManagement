namespace EmployeeManagement.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public const string RoleClaimType = "role";

    public const string NameClaimType = "unique_name";

    private const int MinimumKeyBytes = 32;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SigningKey { get; set; } = string.Empty;

    public int AccessTokenLifetimeMinutes { get; set; } = 60;

    public static bool IsValidSigningKey(string? signingKey)
    {
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            return false;
        }

        var buffer = new byte[signingKey.Length];

        return Convert.TryFromBase64String(signingKey, buffer, out var bytesWritten)
               && bytesWritten >= MinimumKeyBytes;
    }
}
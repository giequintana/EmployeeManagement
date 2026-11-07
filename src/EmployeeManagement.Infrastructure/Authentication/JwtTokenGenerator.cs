using System.Security.Claims;

using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Infrastructure.Authentication;

internal sealed class JwtTokenGenerator(IOptions<JwtOptions> options, TimeProvider timeProvider)
    : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    private readonly SigningCredentials _signingCredentials = new(
        new SymmetricSecurityKey(Convert.FromBase64String(options.Value.SigningKey)),
        SecurityAlgorithms.HmacSha256);

    private readonly JsonWebTokenHandler _tokenHandler = new();

    public AccessToken Generate(ApplicationUser user)
    {
        var issuedAt = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = issuedAt.AddMinutes(_options.AccessTokenLifetimeMinutes);

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtOptions.NameClaimType, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtOptions.RoleClaimType, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            SigningCredentials = _signingCredentials
        };

        return new AccessToken(_tokenHandler.CreateToken(descriptor), expiresAt);
    }
}
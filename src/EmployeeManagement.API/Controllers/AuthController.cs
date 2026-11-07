using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Enums;
using EmployeeManagement.Infrastructure.Authentication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EmployeeManagement.API.Controllers;

[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<AuthenticatedUserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var idValue = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var username = User.Identity?.Name;
        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        var roleValue = User.FindFirst(JwtOptions.RoleClaimType)?.Value;

        if (!int.TryParse(idValue, out var id)
            || string.IsNullOrEmpty(username)
            || string.IsNullOrEmpty(email)
            || !Enum.TryParse<UserRole>(roleValue, out var role))
        {
            return Unauthorized();
        }

        return Ok(new AuthenticatedUserDto(id, username, email, role));
    }
}
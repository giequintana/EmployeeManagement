using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Errors;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;

using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services;

internal sealed class AuthService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork,
    ILogger<AuthService> logger) : IAuthService
{
    private static readonly ApplicationUser UnknownUser = new();

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var user = await users.GetByUsernameAsync(username, cancellationToken);

        if (user is null)
        {
            // Spend the same time as a real verification so response timing
            // doesn't reveal which usernames exist.
            passwordHasher.Verify(UnknownUser, request.Password);
            logger.LogWarning("Failed sign-in: unknown username {Username}", username);
            return AuthErrors.InvalidCredentials;
        }

        var verification = passwordHasher.Verify(user, request.Password);

        if (verification == PasswordVerification.Failed)
        {
            logger.LogWarning("Failed sign-in: wrong password for {Username}", user.Username);
            return AuthErrors.InvalidCredentials;
        }

        if (verification == PasswordVerification.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.Hash(user, request.Password);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Password hash upgraded for {Username}", user.Username);
        }

        var token = tokenGenerator.Generate(user);

        logger.LogInformation("User {Username} signed in with role {Role}", user.Username, user.Role);

        return new LoginResponse(
            token.Token,
            "Bearer",
            token.ExpiresAtUtc,
            new AuthenticatedUserDto(user.Id, user.Username, user.Email, user.Role));
    }
}
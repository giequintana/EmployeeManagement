using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Application.Errors;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Services;

internal sealed class AuthService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork) : IAuthService
{
    private static readonly ApplicationUser UnknownUser = new();

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await users.GetByUsernameAsync(request.Username.Trim(), cancellationToken);

        if (user is null)
        {
            // Spend the same time as a real verification so response timing
            // doesn't reveal which usernames exist.
            passwordHasher.Verify(UnknownUser, request.Password);
            return AuthErrors.InvalidCredentials;
        }

        var verification = passwordHasher.Verify(user, request.Password);

        if (verification == PasswordVerification.Failed)
        {
            return AuthErrors.InvalidCredentials;
        }

        if (verification == PasswordVerification.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.Hash(user, request.Password);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var token = tokenGenerator.Generate(user);

        return new LoginResponse(
            token.Token,
            "Bearer",
            token.ExpiresAtUtc,
            new AuthenticatedUserDto(user.Id, user.Username, user.Email, user.Role));
    }
}
using EmployeeManagement.Core.Enums;

namespace EmployeeManagement.Application.DTOs.Auth;

public sealed record AuthenticatedUserDto(int Id, string Username, string Email, UserRole Role);

public sealed record LoginResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    AuthenticatedUserDto User);
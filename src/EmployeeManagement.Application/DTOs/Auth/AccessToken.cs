namespace EmployeeManagement.Application.DTOs.Auth;

public sealed record AccessToken(string Token, DateTime ExpiresAtUtc);
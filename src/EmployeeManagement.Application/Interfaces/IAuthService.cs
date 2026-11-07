using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Auth;

namespace EmployeeManagement.Application.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
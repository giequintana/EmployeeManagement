using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}
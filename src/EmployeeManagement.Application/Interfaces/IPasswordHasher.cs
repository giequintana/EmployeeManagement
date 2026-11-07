using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Interfaces;

public enum PasswordVerification
{
    Failed,
    Success,
    SuccessRehashNeeded
}

public interface IPasswordHasher
{
    string Hash(ApplicationUser user, string password);

    PasswordVerification Verify(ApplicationUser user, string providedPassword);
}
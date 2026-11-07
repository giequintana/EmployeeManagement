using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;

using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Authentication;

internal sealed class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<ApplicationUser> _hasher = new();
    private readonly string _dummyHash;

    public IdentityPasswordHasher()
    {
        _dummyHash = _hasher.HashPassword(new ApplicationUser(), Guid.NewGuid().ToString());
    }

    public string Hash(ApplicationUser user, string password) =>
        _hasher.HashPassword(user, password);

    public PasswordVerification Verify(ApplicationUser user, string providedPassword)
    {
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            _hasher.VerifyHashedPassword(user, _dummyHash, providedPassword);
            return PasswordVerification.Failed;
        }

        return _hasher.VerifyHashedPassword(user, user.PasswordHash, providedPassword) switch
        {
            PasswordVerificationResult.Success => PasswordVerification.Success,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordVerification.SuccessRehashNeeded,
            _ => PasswordVerification.Failed
        };
    }
}
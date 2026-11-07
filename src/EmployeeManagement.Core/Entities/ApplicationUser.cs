using EmployeeManagement.Core.Enums;

namespace EmployeeManagement.Core.Entities;

public class ApplicationUser : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;
}
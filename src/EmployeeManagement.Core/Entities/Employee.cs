namespace EmployeeManagement.Core.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public DateOnly HireDate { get; set; }

    public string Position { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public bool IsActive { get; set; } = true;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;
}
namespace EmployeeManagement.Application.DTOs.Employees;

public sealed record CreateEmployeeRequest : IEmployeeRequest
{
    public string EmployeeNumber { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? PhoneNumber { get; init; }

    public DateOnly DateOfBirth { get; init; }

    public DateOnly HireDate { get; init; }

    public string Position { get; init; } = string.Empty;

    public decimal Salary { get; init; }

    public int DepartmentId { get; init; }

    public bool IsActive { get; init; } = true;
}
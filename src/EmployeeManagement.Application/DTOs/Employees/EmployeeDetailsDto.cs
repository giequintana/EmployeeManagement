namespace EmployeeManagement.Application.DTOs.Employees;

public sealed record EmployeeDetailsDto(
    int Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateOnly DateOfBirth,
    DateOnly HireDate,
    string Position,
    decimal Salary,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
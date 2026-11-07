namespace EmployeeManagement.Application.DTOs.Employees;

public sealed record EmployeeSummaryDto(
    int Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string Position,
    int DepartmentId,
    string DepartmentName,
    DateOnly HireDate,
    bool IsActive);
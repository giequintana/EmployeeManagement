namespace EmployeeManagement.Application.DTOs.Departments;

public sealed record DepartmentDto(
    int Id,
    string DepartmentName,
    string? Description,
    int EmployeeCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
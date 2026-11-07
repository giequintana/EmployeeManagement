namespace EmployeeManagement.Application.DTOs.Departments;

public sealed record UpdateDepartmentRequest : IDepartmentRequest
{
    public string DepartmentName { get; init; } = string.Empty;

    public string? Description { get; init; }
}
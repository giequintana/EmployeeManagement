namespace EmployeeManagement.Application.DTOs.Departments;

public interface IDepartmentRequest
{
    string DepartmentName { get; }

    string? Description { get; }
}
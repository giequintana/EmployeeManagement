using EmployeeManagement.Application.Common;

namespace EmployeeManagement.Application.Errors;

public static class DepartmentErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Department.NotFound", $"Department with id {id} was not found.");

    public static Error DuplicateName(string departmentName) =>
        Error.Conflict("Department.DuplicateName", $"Department name '{departmentName}' is already in use.");

    public static Error HasEmployees(int id) =>
        Error.Conflict(
            "Department.HasEmployees",
            $"Department with id {id} cannot be deleted because it has assigned employees. Reassign or remove them first.");
}
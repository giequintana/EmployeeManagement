using EmployeeManagement.Application.Common;

namespace EmployeeManagement.Application.Errors;

public static class EmployeeErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Employee.NotFound", $"Employee with id {id} was not found.");

    public static Error DuplicateEmployeeNumber(string employeeNumber) =>
        Error.Conflict("Employee.DuplicateEmployeeNumber", $"Employee number '{employeeNumber}' is already in use.");

    public static Error DuplicateEmail(string email) =>
        Error.Conflict("Employee.DuplicateEmail", $"Email '{email}' is already in use.");

    public static Error DepartmentNotFound(int departmentId) =>
        Error.Validation("Employee.DepartmentNotFound", $"Department with id {departmentId} does not exist.");
}
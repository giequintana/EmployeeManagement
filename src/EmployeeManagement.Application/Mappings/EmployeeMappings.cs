using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Mappings;

internal static class EmployeeMappings
{
    public static Employee ToEntity(this CreateEmployeeRequest request) => new()
    {
        EmployeeNumber = NormalizeEmployeeNumber(request.EmployeeNumber),
        FirstName = request.FirstName.Trim(),
        LastName = request.LastName.Trim(),
        Email = NormalizeEmail(request.Email),
        PhoneNumber = TextNormalization.Optional(request.PhoneNumber),
        DateOfBirth = request.DateOfBirth,
        HireDate = request.HireDate,
        Position = request.Position.Trim(),
        Salary = request.Salary,
        DepartmentId = request.DepartmentId,
        IsActive = request.IsActive
    };

    public static void ApplyTo(this UpdateEmployeeRequest request, Employee employee)
    {
        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = NormalizeEmail(request.Email);
        employee.PhoneNumber = TextNormalization.Optional(request.PhoneNumber);
        employee.DateOfBirth = request.DateOfBirth;
        employee.HireDate = request.HireDate;
        employee.Position = request.Position.Trim();
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;
        employee.IsActive = (bool)request.IsActive;
    }

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    public static string NormalizeEmployeeNumber(string employeeNumber) => employeeNumber.Trim().ToUpperInvariant();

}
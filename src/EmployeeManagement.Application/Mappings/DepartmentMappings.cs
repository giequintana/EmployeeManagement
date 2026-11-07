using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Mappings;

internal static class DepartmentMappings
{
    public static Department ToEntity(this CreateDepartmentRequest request) => new()
    {
        DepartmentName = NormalizeName(request.DepartmentName),
        Description = TextNormalization.Optional(request.Description)
    };

    public static void ApplyTo(this UpdateDepartmentRequest request, Department department)
    {
        department.DepartmentName = NormalizeName(request.DepartmentName);
        department.Description = TextNormalization.Optional(request.Description);
    }

    public static string NormalizeName(string departmentName) => departmentName.Trim();
}
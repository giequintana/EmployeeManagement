using EmployeeManagement.Application.DTOs.Employees;

namespace EmployeeManagement.API.IntegrationTests.Infrastructure;

internal static class TestData
{
    public static string Unique() => Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

    public static CreateEmployeeRequest NewEmployee(int departmentId = 1)
    {
        var suffix = Unique();

        return new CreateEmployeeRequest
        {
            EmployeeNumber = $"T-{suffix}",
            FirstName = "Test",
            LastName = $"Employee{suffix}",
            Email = $"t{suffix.ToLowerInvariant()}@example.com",
            PhoneNumber = "0917 555 0101",
            DateOfBirth = new DateOnly(1990, 1, 15),
            HireDate = new DateOnly(2020, 6, 1),
            Position = "Tester",
            Salary = 50000m,
            DepartmentId = departmentId,
            IsActive = true
        };
    }
}
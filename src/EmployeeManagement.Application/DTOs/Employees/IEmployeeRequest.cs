namespace EmployeeManagement.Application.DTOs.Employees;

public interface IEmployeeRequest
{
    string FirstName { get; }

    string LastName { get; }

    string Email { get; }

    string? PhoneNumber { get; }

    DateOnly DateOfBirth { get; }

    DateOnly HireDate { get; }

    string Position { get; }

    decimal Salary { get; }

    int DepartmentId { get; }
}
using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<PagedResult<EmployeeSummaryDto>> GetPagedAsync(
        EmployeeQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<EmployeeDetailsDto?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null, CancellationToken cancellationToken = default);

    void Add(Employee employee);

    void Remove(Employee employee);
}
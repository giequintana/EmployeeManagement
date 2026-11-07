using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeSummaryDto>> GetPagedAsync(
        EmployeeQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<Result<EmployeeDetailsDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<EmployeeDetailsDto>> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task<Result<EmployeeDetailsDto>> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
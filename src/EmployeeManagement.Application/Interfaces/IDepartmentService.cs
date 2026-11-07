using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.DTOs.Departments;

namespace EmployeeManagement.Application.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<DepartmentDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);

    Task<Result<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
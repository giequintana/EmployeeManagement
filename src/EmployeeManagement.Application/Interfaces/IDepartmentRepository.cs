using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DepartmentDto?> GetDtoByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(string departmentName, int? excludeDepartmentId = null, CancellationToken cancellationToken = default);

    Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken = default);

    void Add(Department department);

    void Remove(Department department);
}
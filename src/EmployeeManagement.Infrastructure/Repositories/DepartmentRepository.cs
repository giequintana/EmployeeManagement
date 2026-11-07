using System.Linq.Expressions;
using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

internal sealed class DepartmentRepository(ApplicationDbContext context) : IDepartmentRepository
{
    private static readonly Expression<Func<Department, DepartmentDto>> ToDto =
        d => new DepartmentDto(
            d.Id,
            d.DepartmentName,
            d.Description,
            d.Employees.Count(),
            d.CreatedAt,
            d.UpdatedAt);

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Departments
            .OrderBy(d => d.DepartmentName)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

    public Task<DepartmentDto?> GetDtoByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Departments
            .Where(d => d.Id == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.Departments.AnyAsync(d => d.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(
        string departmentName,
        int? excludeDepartmentId = null,
        CancellationToken cancellationToken = default) =>
        context.Departments.AnyAsync(
            d => d.DepartmentName == departmentName && (excludeDepartmentId == null || d.Id != excludeDepartmentId),
            cancellationToken);

    public Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken = default) =>
        context.Employees.AnyAsync(e => e.DepartmentId == id, cancellationToken);

    public void Add(Department department) => context.Departments.Add(department);

    public void Remove(Department department) => context.Departments.Remove(department);
}
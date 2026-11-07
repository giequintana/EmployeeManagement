using System.Linq.Expressions;
using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

internal sealed class EmployeeRepository(ApplicationDbContext context) : IEmployeeRepository
{
    private static readonly Expression<Func<Employee, EmployeeSummaryDto>> ToSummaryDto =
        e => new EmployeeSummaryDto(
            e.Id,
            e.EmployeeNumber,
            e.FirstName,
            e.LastName,
            e.Email,
            e.Position,
            e.DepartmentId,
            e.Department.DepartmentName,
            e.HireDate,
            e.IsActive);

    private static readonly Expression<Func<Employee, EmployeeDetailsDto>> ToDetailsDto =
        e => new EmployeeDetailsDto(
            e.Id,
            e.EmployeeNumber,
            e.FirstName,
            e.LastName,
            e.Email,
            e.PhoneNumber,
            e.DateOfBirth,
            e.HireDate,
            e.Position,
            e.Salary,
            e.DepartmentId,
            e.Department.DepartmentName,
            e.IsActive,
            e.CreatedAt,
            e.UpdatedAt);

    public async Task<PagedResult<EmployeeSummaryDto>> GetPagedAsync(
        EmployeeQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(context.Employees, parameters);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await ApplySorting(query, parameters.SortBy, parameters.SortDescending)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(ToSummaryDto)
            .ToListAsync(cancellationToken);

        return new PagedResult<EmployeeSummaryDto>(
            items,
            parameters.PageNumber,
            parameters.PageSize,
            totalCount);
    }

    public Task<EmployeeDetailsDto?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Employees
            .Where(e => e.Id == id)
            .Select(ToDetailsDto)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken cancellationToken = default) =>
        context.Employees.AnyAsync(e => e.EmployeeNumber == employeeNumber, cancellationToken);

    public Task<bool> EmailExistsAsync(
        string email,
        int? excludeEmployeeId = null,
        CancellationToken cancellationToken = default) =>
        context.Employees.AnyAsync(
            e => e.Email == email && (excludeEmployeeId == null || e.Id != excludeEmployeeId),
            cancellationToken);

    public void Add(Employee employee) => context.Employees.Add(employee);

    public void Remove(Employee employee) => context.Employees.Remove(employee);

    private static IQueryable<Employee> ApplyFilters(IQueryable<Employee> query, EmployeeQueryParameters parameters)
    {
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var term = parameters.Search.Trim();

            query = query.Where(e =>
                e.EmployeeNumber.Contains(term) ||
                e.FirstName.Contains(term) ||
                e.LastName.Contains(term) ||
                (e.FirstName + " " + e.LastName).Contains(term));
        }

        if (parameters.DepartmentId.HasValue)
        {
            var departmentId = parameters.DepartmentId.Value;
            query = query.Where(e => e.DepartmentId == departmentId);
        }

        if (parameters.IsActive.HasValue)
        {
            var isActive = parameters.IsActive.Value;
            query = query.Where(e => e.IsActive == isActive);
        }

        return query;
    }

    private static IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, bool descending)
    {
        IOrderedQueryable<Employee> ordered = sortBy?.Trim().ToLowerInvariant() switch
        {
            "employeenumber" => descending
                ? query.OrderByDescending(e => e.EmployeeNumber)
                : query.OrderBy(e => e.EmployeeNumber),
            "firstname" => descending
                ? query.OrderByDescending(e => e.FirstName)
                : query.OrderBy(e => e.FirstName),
            "hiredate" => descending
                ? query.OrderByDescending(e => e.HireDate)
                : query.OrderBy(e => e.HireDate),
            "position" => descending
                ? query.OrderByDescending(e => e.Position)
                : query.OrderBy(e => e.Position),
            "department" => descending
                ? query.OrderByDescending(e => e.Department.DepartmentName)
                : query.OrderBy(e => e.Department.DepartmentName),
            _ => descending
                ? query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName)
                : query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
        };

        return ordered.ThenBy(e => e.Id);
    }
}
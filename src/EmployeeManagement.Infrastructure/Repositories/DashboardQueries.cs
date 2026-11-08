using EmployeeManagement.Application.DTOs.Dashboard;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

internal sealed class DashboardQueries(ApplicationDbContext context) : IDashboardQueries
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var distribution = await context.Departments
            .OrderBy(d => d.DepartmentName)
            .Select(d => new DepartmentDistributionDto(
                d.Id,
                d.DepartmentName,
                d.Employees.Count(),
                d.Employees.Count(e => e.IsActive)))
            .ToListAsync(cancellationToken);

        var totalEmployees = distribution.Sum(d => d.EmployeeCount);
        var activeEmployees = distribution.Sum(d => d.ActiveEmployeeCount);

        return new DashboardSummaryDto(
            totalEmployees,
            activeEmployees,
            totalEmployees - activeEmployees,
            distribution.Count,
            distribution);
    }
}
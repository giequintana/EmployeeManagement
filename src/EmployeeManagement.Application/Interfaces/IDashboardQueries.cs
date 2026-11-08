using EmployeeManagement.Application.DTOs.Dashboard;

namespace EmployeeManagement.Application.Interfaces;

public interface IDashboardQueries
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
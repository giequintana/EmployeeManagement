using EmployeeManagement.Application.DTOs.Dashboard;
using EmployeeManagement.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers;

[Route("api/dashboard")]
public sealed class DashboardController(IDashboardQueries dashboardQueries) : ApiControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType<DashboardSummaryDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await dashboardQueries.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }
}
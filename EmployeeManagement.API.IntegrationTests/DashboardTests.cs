using System.Collections;
using System.Net;

using EmployeeManagement.API.IntegrationTests.Infrastructure;
using EmployeeManagement.Application.DTOs.Dashboard;

namespace EmployeeManagement.API.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class DashboardTests(ApiFixture fixture)
{
    [Fact]
    public async Task Summary_TotalsAreConsistentWithDistribution()
    {
        using var user = await fixture.CreateClientAsync(TestUser.User);

        var response = await user.GetAsync("/api/dashboard/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = await TestJson.ReadAsync<DashboardSummaryDto>(response);
        Assert.Equal(summary.EmployeesByDepartment.Sum(d => d.EmployeeCount), summary.TotalEmployees);
        Assert.Equal(summary.TotalEmployees, summary.ActiveEmployees + summary.InactiveEmployees);
        Assert.Equal(summary.EmployeesByDepartment.Count, summary.TotalDepartments);
    }
}
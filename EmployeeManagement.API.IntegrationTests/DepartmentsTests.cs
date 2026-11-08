using System.Collections;
using System.Net;
using System.Net.Http.Json;

using EmployeeManagement.API.IntegrationTests.Infrastructure;
using EmployeeManagement.Application.DTOs.Departments;
using EmployeeManagement.Application.DTOs.Employees;

namespace EmployeeManagement.API.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class DepartmentsTests(ApiFixture fixture)
{
    [Fact]
    public async Task GetAll_IncludesSeededDepartments()
    {
        using var user = await fixture.CreateClientAsync(TestUser.User);

        var departments = await TestJson.ReadAsync<List<DepartmentDto>>(await user.GetAsync("/api/departments"));

        var names = departments.Select(d => d.DepartmentName).ToArray();
        Assert.Contains("Human Resources", names);
        Assert.Contains("Information Technology", names);
        Assert.Contains("Finance", names);
    }

    [Fact]
    public async Task Create_WithDuplicateNameInDifferentCase_Returns409()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);

        var response = await admin.PostAsJsonAsync("/api/departments",
            new CreateDepartmentRequest { DepartmentName = "finance" }, TestJson.Options);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("Department.DuplicateName", await TestJson.ReadErrorCodeAsync(response));
    }

    [Fact]
    public async Task Delete_DepartmentWithEmployees_Returns409_ThenSucceedsWhenEmpty()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);

        var department = await TestJson.ReadAsync<DepartmentDto>(await admin.PostAsJsonAsync("/api/departments",
            new CreateDepartmentRequest { DepartmentName = $"Test Dept {TestData.Unique()}" }, TestJson.Options));

        var employee = await TestJson.ReadAsync<EmployeeDetailsDto>(await admin.PostAsJsonAsync("/api/employees",
            TestData.NewEmployee(department.Id), TestJson.Options));

        var blocked = await admin.DeleteAsync($"/api/departments/{department.Id}");
        Assert.Equal(HttpStatusCode.Conflict, blocked.StatusCode);
        Assert.Equal("Department.HasEmployees", await TestJson.ReadErrorCodeAsync(blocked));

        await admin.DeleteAsync($"/api/employees/{employee.Id}");

        var allowed = await admin.DeleteAsync($"/api/departments/{department.Id}");
        Assert.Equal(HttpStatusCode.NoContent, allowed.StatusCode);
    }
}
using System.Net;
using System.Net.Http.Json;
using System.Globalization;
using System.Text.Json;

using EmployeeManagement.API.IntegrationTests.Infrastructure;
using EmployeeManagement.Application.DTOs.Common;
using EmployeeManagement.Application.DTOs.Employees;

namespace EmployeeManagement.API.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class EmployeesTests(ApiFixture fixture)
{
    [Fact]
    public async Task Employee_FullLifecycle_CreateReadUpdateDelete()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);
        var request = TestData.NewEmployee();

        // Create
        var createResponse = await admin.PostAsJsonAsync("/api/employees", request, TestJson.Options);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await TestJson.ReadAsync<EmployeeDetailsDto>(createResponse);
        Assert.EndsWith($"/api/employees/{created.Id}", createResponse.Headers.Location?.ToString());
        Assert.Equal(request.EmployeeNumber, created.EmployeeNumber);
        Assert.True(created.IsActive);
        Assert.Null(created.UpdatedAt);

        // Read
        var getResponse = await admin.GetAsync($"/api/employees/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // Update (deactivate)
        var update = new UpdateEmployeeRequest
        {
            FirstName = created.FirstName,
            LastName = created.LastName,
            Email = created.Email,
            PhoneNumber = created.PhoneNumber,
            DateOfBirth = created.DateOfBirth,
            HireDate = created.HireDate,
            Position = "Senior Tester",
            Salary = 60000m,
            DepartmentId = created.DepartmentId,
            IsActive = false
        };

        var updateResponse = await admin.PutAsJsonAsync($"/api/employees/{created.Id}", update, TestJson.Options);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await TestJson.ReadAsync<EmployeeDetailsDto>(updateResponse);
        Assert.Equal("Senior Tester", updated.Position);
        Assert.False(updated.IsActive);
        Assert.NotNull(updated.UpdatedAt);
        Assert.Equal(created.CreatedAt, updated.CreatedAt);

        // Delete
        var deleteResponse = await admin.DeleteAsync($"/api/employees/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDelete = await admin.GetAsync($"/api/employees/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
        Assert.Equal("Employee.NotFound", await TestJson.ReadErrorCodeAsync(afterDelete));
    }

    [Fact]
    public async Task Create_WithDuplicateEmployeeNumberInDifferentCase_Returns409()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);
        var first = TestData.NewEmployee();
        await admin.PostAsJsonAsync("/api/employees", first, TestJson.Options);

        var duplicate = TestData.NewEmployee() with { EmployeeNumber = first.EmployeeNumber.ToLowerInvariant() };
        var response = await admin.PostAsJsonAsync("/api/employees", duplicate, TestJson.Options);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("Employee.DuplicateEmployeeNumber", await TestJson.ReadErrorCodeAsync(response));
    }

    [Fact]
    public async Task Create_WithEmptyBody_Returns400WithFieldErrors()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);

        var response = await admin.PostAsJsonAsync("/api/employees", new { }, TestJson.Options);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var fields = await TestJson.ReadValidationFieldsAsync(response);
        Assert.Contains("employeeNumber", fields);
        Assert.Contains("firstName", fields);
        Assert.Contains("email", fields);
        Assert.Contains("dateOfBirth", fields);
        Assert.Contains("departmentId", fields);
    }

    [Fact]
    public async Task Update_WithoutIsActive_Returns400()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);
        var body = new
        {
            firstName = "Test",
            lastName = "Employee",
            email = "missing.isactive@example.com",
            dateOfBirth = "1990-01-15",
            hireDate = "2020-06-01",
            position = "Tester",
            salary = 50000,
            departmentId = 1
        };

        var response = await admin.PutAsJsonAsync("/api/employees/999999", body, TestJson.Options);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("isActive", await TestJson.ReadValidationFieldsAsync(response));
    }

    [Fact]
    public async Task Create_AsRegularUser_Returns403()
    {
        using var user = await fixture.CreateClientAsync(TestUser.User);

        var response = await user.PostAsJsonAsync("/api/employees", TestData.NewEmployee(), TestJson.Options);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetPaged_ClampsPageSizeAndFindsBySearch()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);
        var request = TestData.NewEmployee();
        await admin.PostAsJsonAsync("/api/employees", request, TestJson.Options);

        var clamped = await TestJson.ReadAsync<PagedResult<EmployeeSummaryDto>>(
            await admin.GetAsync("/api/employees?pageSize=1000"));
        Assert.Equal(50, clamped.PageSize);

        var search = await TestJson.ReadAsync<PagedResult<EmployeeSummaryDto>>(
            await admin.GetAsync($"/api/employees?search={request.EmployeeNumber.ToLowerInvariant()}"));
        var match = Assert.Single(search.Items);
        Assert.Equal(request.EmployeeNumber, match.EmployeeNumber);
    }

    [Fact]
    public async Task Create_ReturnsTimestampsAsUtc()
    {
        using var admin = await fixture.CreateClientAsync(TestUser.Admin);

        var response = await admin.PostAsJsonAsync("/api/employees", TestData.NewEmployee(), TestJson.Options);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var createdAt = document.RootElement.GetProperty("createdAt").GetString();

        Assert.NotNull(createdAt);
        Assert.EndsWith("Z", createdAt);

        var parsed = DateTimeOffset.Parse(createdAt, CultureInfo.InvariantCulture);
        Assert.InRange(parsed, DateTimeOffset.UtcNow.AddMinutes(-2), DateTimeOffset.UtcNow.AddMinutes(1));
    }
}
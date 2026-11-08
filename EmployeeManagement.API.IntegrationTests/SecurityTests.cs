using EmployeeManagement.API.IntegrationTests.Infrastructure;

namespace EmployeeManagement.API.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class SecurityTests(ApiFixture fixture)
{
    [Fact]
    public async Task Responses_IncludeSecurityHeaders_EvenOnErrors()
    {
        using var client = fixture.CreateAnonymousClient();

        var response = await client.GetAsync("/api/employees"); // 401, still gets the headers

        Assert.Equal("nosniff", Assert.Single(response.Headers.GetValues("X-Content-Type-Options")));
        Assert.Equal("DENY", Assert.Single(response.Headers.GetValues("X-Frame-Options")));
        Assert.Equal("no-referrer", Assert.Single(response.Headers.GetValues("Referrer-Policy")));
        Assert.True(response.Headers.CacheControl?.NoStore);
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
    }
}
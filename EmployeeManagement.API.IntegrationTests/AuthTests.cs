using System.Net;
using System.Net.Http.Json;

using EmployeeManagement.API.IntegrationTests.Infrastructure;
using EmployeeManagement.Application.DTOs.Auth;
using EmployeeManagement.Core.Enums;

namespace EmployeeManagement.API.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class AuthTests(ApiFixture fixture)
{
    [Fact]
    public async Task Login_WithValidAdminCredentials_ReturnsTokenAndAdminRole()
    {
        using var client = fixture.CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Username = ApiFactory.AdminUsername, Password = ApiFactory.AdminPassword },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var login = await TestJson.ReadAsync<LoginResponse>(response);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.Equal("Bearer", login.TokenType);
        Assert.Equal(UserRole.Admin, login.User.Role);
    }

    [Fact]
    public async Task Login_WithWrongPasswordOrUnknownUser_ReturnsSameUnauthorizedError()
    {
        using var client = fixture.CreateAnonymousClient();

        var wrongPassword = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Username = ApiFactory.AdminUsername, Password = "not-the-password" },
            TestJson.Options);
        var unknownUser = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Username = "nobody", Password = "not-the-password" },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unknownUser.StatusCode);
        Assert.Equal("Auth.InvalidCredentials", await TestJson.ReadErrorCodeAsync(wrongPassword));
        Assert.Equal("Auth.InvalidCredentials", await TestJson.ReadErrorCodeAsync(unknownUser));
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        using var client = fixture.CreateAnonymousClient();

        var response = await client.GetAsync("/api/employees");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithUserToken_ReturnsUserRole()
    {
        using var client = await fixture.CreateClientAsync(TestUser.User);

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var me = await TestJson.ReadAsync<AuthenticatedUserDto>(response);
        Assert.Equal(ApiFactory.UserUsername, me.Username);
        Assert.Equal(UserRole.User, me.Role);
    }
}
using System.Net.Http.Headers;
using System.Net.Http.Json;

using EmployeeManagement.Application.DTOs.Auth;

using Microsoft.AspNetCore.Mvc.Testing;

namespace EmployeeManagement.API.IntegrationTests.Infrastructure;

public enum TestUser
{
    Admin,
    User
}

public sealed class ApiFixture : IDisposable
{
    private readonly Dictionary<TestUser, string> _tokens = [];

    public ApiFixture()
    {
        Factory = new ApiFactory();
        Factory.ResetDatabase();
    }

    public ApiFactory Factory { get; }

    public HttpClient CreateAnonymousClient() =>
        Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

    public async Task<HttpClient> CreateClientAsync(TestUser user)
    {
        var client = CreateAnonymousClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await GetTokenAsync(user));
        return client;
    }

    private async Task<string> GetTokenAsync(TestUser user)
    {
        if (_tokens.TryGetValue(user, out var cached))
        {
            return cached;
        }

        var (username, password) = user == TestUser.Admin
            ? (ApiFactory.AdminUsername, ApiFactory.AdminPassword)
            : (ApiFactory.UserUsername, ApiFactory.UserPassword);

        using var client = CreateAnonymousClient();
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest { Username = username, Password = password },
            TestJson.Options);

        response.EnsureSuccessStatusCode();

        var login = await TestJson.ReadAsync<LoginResponse>(response);
        _tokens[user] = login.AccessToken;

        return login.AccessToken;
    }

    public void Dispose() => Factory.Dispose();
}
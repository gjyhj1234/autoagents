using System.Net;
using System.Net.Http.Json;
using DentalChart.Api.Contracts.Auth;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DentalChart.Tests.Auth;

public sealed class AuthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        var request = new LoginRequest("nonexistent", "wrongpassword");
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_Returns401()
    {
        var request = new RefreshRequest("invalid-token-value");
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutAuth_Returns401()
    {
        var response = await _client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithoutAuth_Returns401()
    {
        var request = new ChangePasswordRequest("old", "new");
        var response = await _client.PostAsJsonAsync("/api/auth/change-password", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

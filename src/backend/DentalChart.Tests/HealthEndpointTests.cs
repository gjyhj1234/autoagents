using System.Net.Http.Json;
using DentalChart.Api.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DentalChart.Tests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthReturnsHealthyPayload()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(payload);
        Assert.Equal("healthy", payload.Status);
        Assert.True(payload.Timestamp > DateTimeOffset.MinValue);
    }
}

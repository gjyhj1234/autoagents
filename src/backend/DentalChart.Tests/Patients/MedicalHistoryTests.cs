using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DentalChart.Tests.Patients;

public sealed class MedicalHistoryTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MedicalHistoryTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMedicalHistory_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/patients/{Guid.NewGuid()}/medical-history");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpsertMedicalHistory_WithoutAuth_Returns401()
    {
        var response = await _client.PutAsJsonAsync($"/api/patients/{Guid.NewGuid()}/medical-history", new { });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

using System.Net;
using System.Net.Http.Json;
using DentalChart.Api.Contracts.Patients;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DentalChart.Tests.Patients;

public sealed class PatientApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PatientApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPatients_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/patients");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPatient_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/patients/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePatient_WithoutAuth_Returns401()
    {
        var request = new CreatePatientRequest("Jane", "Doe", "1990-05-20");
        var response = await _client.PostAsJsonAsync("/api/patients", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletePatient_WithoutAuth_Returns401()
    {
        var response = await _client.DeleteAsync($"/api/patients/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

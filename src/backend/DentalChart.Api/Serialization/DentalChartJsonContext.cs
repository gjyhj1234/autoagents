using System.Text.Json.Serialization;
using DentalChart.Api.Contracts;

namespace DentalChart.Api.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(HealthResponse))]
internal sealed partial class DentalChartJsonContext : JsonSerializerContext
{
}

namespace DentalChart.Api.Contracts;

public sealed record ErrorResponse(string Error, string? Message = null, object? Details = null);

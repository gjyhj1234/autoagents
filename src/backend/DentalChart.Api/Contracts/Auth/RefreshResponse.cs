namespace DentalChart.Api.Contracts.Auth;

public sealed record RefreshResponse(string AccessToken, int ExpiresIn);

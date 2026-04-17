namespace DentalChart.Api.Contracts.Auth;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserDto User);

public sealed record UserDto(Guid Id, string Username, string Role, string Name);

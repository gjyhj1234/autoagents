namespace DentalChart.Infrastructure;

public sealed class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "DentalChart.Api";
    public string Audience { get; set; } = "DentalChart.Frontend";
    public int AccessTokenExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}

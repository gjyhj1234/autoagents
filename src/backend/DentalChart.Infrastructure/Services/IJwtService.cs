using DentalChart.Core.Entities;

namespace DentalChart.Infrastructure.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? ValidateRefreshToken(string token);
}

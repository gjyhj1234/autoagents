using System.Security.Cryptography;
using System.Text;
using DentalChart.Api.Contracts;
using DentalChart.Api.Contracts.Auth;
using DentalChart.Core.Entities;
using DentalChart.Infrastructure.Data;
using DentalChart.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace DentalChart.Api.Endpoints;

public static class AuthEndpoints
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 30;

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync);
        group.MapPost("/refresh", RefreshAsync);
        group.MapPost("/logout", LogoutAsync).RequireAuthorization();
        group.MapPost("/change-password", ChangePasswordAsync).RequireAuthorization();

        return routes;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        DentalChartDbContext db,
        IJwtService jwtService)
    {
        var user = await db.Users
            .Where(u => u.Username == request.Username && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user is null)
            return TypedResults.Json(
                new ErrorResponse("InvalidCredentials", "Username or password is incorrect"),
                statusCode: 401);

        // Check lockout
        if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTimeOffset.UtcNow)
            return TypedResults.Json(
                new ErrorResponse("AccountLocked", $"Account locked until {user.LockedUntil.Value:O}"),
                statusCode: 423);

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockedUntil = DateTimeOffset.UtcNow.AddMinutes(LockoutMinutes);
            }
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync();

            return TypedResults.Json(
                new ErrorResponse("InvalidCredentials", "Username or password is incorrect"),
                statusCode: 401);
        }

        // Reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        user.LastLogin = DateTimeOffset.UtcNow;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        // Issue tokens
        var accessToken = jwtService.GenerateAccessToken(user);
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var tokenHash = HashToken(rawRefreshToken);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            CreatedAt = DateTimeOffset.UtcNow,
        };
        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        var provider = user.Provider;
        var name = provider is not null
            ? $"{provider.FirstName} {provider.LastName}"
            : user.Username;

        return TypedResults.Ok(new LoginResponse(
            accessToken,
            rawRefreshToken,
            3600,
            new UserDto(user.Id, user.Username, user.Role, name)));
    }

    private static async Task<IResult> RefreshAsync(
        RefreshRequest request,
        DentalChartDbContext db,
        IJwtService jwtService)
    {
        var tokenHash = HashToken(request.RefreshToken);

        var storedToken = await db.RefreshTokens
            .Include(rt => rt.User)
            .Where(rt => rt.TokenHash == tokenHash && !rt.IsRevoked && rt.ExpiresAt > DateTimeOffset.UtcNow)
            .FirstOrDefaultAsync();

        if (storedToken is null || storedToken.User is null)
            return TypedResults.Json(new ErrorResponse("InvalidRefreshToken"), statusCode: 401);

        // Revoke old token (rotation)
        storedToken.IsRevoked = true;
        storedToken.RevokedReason = "rotated";

        // Issue new access token
        var accessToken = jwtService.GenerateAccessToken(storedToken.User);

        // Issue new refresh token
        var rawRefreshToken = jwtService.GenerateRefreshToken();
        var newTokenHash = HashToken(rawRefreshToken);
        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            TokenHash = newTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await db.SaveChangesAsync();

        return TypedResults.Ok(new RefreshResponse(accessToken, 3600));
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext context,
        DentalChartDbContext db)
    {
        // Revoke all refresh tokens for the user
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?? context.User.FindFirst("sub");
        if (userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            await db.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(rt => rt.IsRevoked, true)
                    .SetProperty(rt => rt.RevokedReason, "logout"));
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> ChangePasswordAsync(
        ChangePasswordRequest request,
        HttpContext context,
        DentalChartDbContext db)
    {
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?? context.User.FindFirst("sub");
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return TypedResults.Unauthorized();

        var user = await db.Users.FindAsync(userId);
        if (user is null)
            return TypedResults.Unauthorized();

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return TypedResults.Json(
                new ErrorResponse("InvalidPassword", "Current password is incorrect"),
                statusCode: 400);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Infrastructure.Identity;
using SponsorshipWorkflow.Infrastructure.Persistence;

namespace SponsorshipWorkflow.Infrastructure.Services;

public class TokenService(IConfiguration configuration, ApplicationDbContext db) : ITokenService
{
    public (string Token, DateTime ExpiresAt) GenerateAccessToken(string userId, string email, string fullName, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new("FullName", fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var secret = configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");

        if (!double.TryParse(configuration["JwtSettings:ExpiryMinutes"] ?? "5", out var expiryMinutes))
            throw new InvalidOperationException("JwtSettings:ExpiryMinutes must be a valid number.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public async Task<(string Token, DateTime ExpiresAt)> IssueRefreshTokenAsync(
        string userId, CancellationToken cancellationToken = default)
    {
        var expiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpiryDays());
        var plaintext = GenerateSecureToken();

        db.RefreshTokens.Add(new RefreshToken(userId, HashToken(plaintext), expiresAt));
        await db.SaveChangesAsync(cancellationToken);

        return (plaintext, expiresAt);
    }

    public async Task<(bool IsValid, string? UserId, string? NewRefreshToken, DateTime? RefreshTokenExpiresAt)> RefreshAsync(
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var hash = HashToken(refreshToken);
        var existing = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (existing == null || !existing.IsActive)
            return (false, null, null, null);

        existing.Revoke();

        var expiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpiryDays());
        var newPlaintext = GenerateSecureToken();
        db.RefreshTokens.Add(new RefreshToken(existing.UserId, HashToken(newPlaintext), expiresAt));

        await db.SaveChangesAsync(cancellationToken);

        return (true, existing.UserId, newPlaintext, expiresAt);
    }

    public async Task RevokeAllRefreshTokensAsync(string userId, CancellationToken cancellationToken = default)
    {
        var tokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
            token.Revoke();

        await db.SaveChangesAsync(cancellationToken);
    }

    private int GetRefreshTokenExpiryDays()
    {
        if (!int.TryParse(configuration["JwtSettings:RefreshTokenExpiryDays"] ?? "10", out var days))
            throw new InvalidOperationException("JwtSettings:RefreshTokenExpiryDays must be a valid number.");
        return days;
    }

    private static string GenerateSecureToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}

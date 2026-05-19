namespace SponsorshipWorkflow.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(string userId, string email, string fullName, IList<string> roles);

    Task<(string Token, DateTime ExpiresAt)> IssueRefreshTokenAsync(string userId, CancellationToken cancellationToken = default);

    Task<(bool IsValid, string? UserId, string? NewRefreshToken, DateTime? RefreshTokenExpiresAt)> RefreshAsync(
        string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeAllRefreshTokensAsync(string userId, CancellationToken cancellationToken = default);
}

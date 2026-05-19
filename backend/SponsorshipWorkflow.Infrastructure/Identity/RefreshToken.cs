namespace SponsorshipWorkflow.Infrastructure.Identity;

public class RefreshToken
{
    private RefreshToken() { }

    public RefreshToken(string userId, string tokenHash, DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserId { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsRevoked { get; private set; }

    public bool IsActive => !IsRevoked && DateTime.UtcNow <= ExpiresAt;

    public void Revoke() => IsRevoked = true;

    public ApplicationUser User { get; private set; } = null!;
}

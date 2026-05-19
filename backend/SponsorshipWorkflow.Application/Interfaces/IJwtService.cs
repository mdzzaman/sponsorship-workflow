namespace SponsorshipWorkflow.Application.Interfaces;

public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateToken(string userId, string email, string fullName, IList<string> roles);
}

namespace SponsorshipWorkflow.API.Contracts.Auth;

public record LoginResponse(
    string Token,
    string UserId,
    string Email,
    string FullName,
    string Role,
    DateTime ExpiresAt
);

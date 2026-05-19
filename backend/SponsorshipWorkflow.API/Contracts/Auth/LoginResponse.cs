namespace SponsorshipWorkflow.API.Contracts.Auth;

public record LoginResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    string UserId,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles
);

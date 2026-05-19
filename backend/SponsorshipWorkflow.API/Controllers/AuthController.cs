using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.API.Contracts.Auth;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Infrastructure.Identity;

namespace SponsorshipWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { error = "Invalid email or password." });

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessTokenExpiresAt) = tokenService.GenerateAccessToken(user.Id, user.Email!, user.FullName, roles);
        var (refreshToken, refreshTokenExpiresAt) = await tokenService.IssueRefreshTokenAsync(user.Id, cancellationToken);

        return Ok(new LoginResponse(
            AccessToken: accessToken,
            AccessTokenExpiresAt: accessTokenExpiresAt,
            RefreshToken: refreshToken,
            RefreshTokenExpiresAt: refreshTokenExpiresAt,
            UserId: user.Id,
            Email: user.Email!,
            FullName: user.FullName,
            Roles: [.. roles]
        ));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(new { error = "Refresh token is required." });

        var (isValid, userId, newRefreshToken, refreshTokenExpiresAt) =
            await tokenService.RefreshAsync(request.RefreshToken, cancellationToken);

        if (!isValid || userId == null || newRefreshToken == null)
            return Unauthorized(new { error = "Invalid or expired refresh token." });

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized(new { error = "User not found." });

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessTokenExpiresAt) = tokenService.GenerateAccessToken(user.Id, user.Email!, user.FullName, roles);

        return Ok(new TokenResponse(
            AccessToken: accessToken,
            AccessTokenExpiresAt: accessTokenExpiresAt,
            RefreshToken: newRefreshToken,
            RefreshTokenExpiresAt: refreshTokenExpiresAt!.Value
        ));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        await tokenService.RevokeAllRefreshTokensAsync(userId, cancellationToken);
        return Ok();
    }
}

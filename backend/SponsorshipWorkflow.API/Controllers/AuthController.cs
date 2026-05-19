using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.API.Contracts.Auth;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Infrastructure.Identity;

namespace SponsorshipWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { error = "Invalid email or password." });

        var roles = await userManager.GetRolesAsync(user);
        var (token, expiresAt) = jwtService.GenerateToken(user.Id, user.Email!, user.FullName, roles);

        return Ok(new LoginResponse(
            Token: token,
            UserId: user.Id,
            Email: user.Email!,
            FullName: user.FullName,
            Roles: [.. roles],
            ExpiresAt: expiresAt
        ));
    }
}

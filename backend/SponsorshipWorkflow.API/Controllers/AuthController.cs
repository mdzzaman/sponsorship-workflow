using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SponsorshipWorkflow.API.Contracts.Auth;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<IdentityUser> userManager, IJwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { error = "Invalid email or password." });

        var roles = await userManager.GetRolesAsync(user);
        var claims = await userManager.GetClaimsAsync(user);
        var fullName = claims.FirstOrDefault(c => c.Type == "FullName")?.Value ?? user.Email!;

        var token = await jwtService.GenerateTokenAsync(user.Id, user.Email!, roles);

        return Ok(new LoginResponse(
            Token: token,
            UserId: user.Id,
            Email: user.Email!,
            FullName: fullName,
            Role: roles.FirstOrDefault() ?? string.Empty,
            ExpiresAt: DateTime.UtcNow.AddHours(8)
        ));
    }
}

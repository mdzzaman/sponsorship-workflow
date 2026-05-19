using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser(
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<ApplicationDbContextInitialiser> logger)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedSponsorshipTypesAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roles = [UserRole.Requestor, UserRole.Manager, UserRole.FinanceAdmin, UserRole.SystemAdmin];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            (Email: "requestor@test.com", FullName: "Alice Requestor", Role: UserRole.Requestor),
            (Email: "manager@test.com",   FullName: "Bob Manager",     Role: UserRole.Manager),
            (Email: "finance@test.com",   FullName: "Carol Finance",   Role: UserRole.FinanceAdmin),
            (Email: "admin@test.com",     FullName: "Dave Admin",      Role: UserRole.SystemAdmin),
        };

        foreach (var (email, fullName, role) in users)
        {
            if (await userManager.FindByEmailAsync(email) != null) continue;

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Test@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", fullName));
            }
        }
    }

    private async Task SeedSponsorshipTypesAsync()
    {
        if (await context.SponsorshipTypes.AnyAsync()) return;

        var types = new[]
        {
            "Conference Sponsorship",
            "Community Event Sponsorship",
            "Sports Event Sponsorship",
            "Educational Program Sponsorship",
            "Charity/CSR Sponsorship"
        };

        foreach (var name in types)
            context.SponsorshipTypes.Add(new SponsorshipType(name));

        await context.SaveChangesAsync();
    }
}

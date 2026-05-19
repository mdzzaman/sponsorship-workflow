using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;
using SponsorshipWorkflow.Infrastructure.Identity;

namespace SponsorshipWorkflow.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILogger<ApplicationDbContextInitialiser> logger,
    IConfiguration configuration)
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
        var roles = new[]
        {
            (Name: UserRole.Requestor,   Responsibility: "Submit and manage sponsorship requests"),
            (Name: UserRole.Manager,     Responsibility: "Review and approve requests at the first level"),
            (Name: UserRole.FinanceAdmin, Responsibility: "Review and approve requests at the finance level"),
            (Name: UserRole.SystemAdmin, Responsibility: "Manage system configuration and users"),
        };

        foreach (var (name, responsibility) in roles)
        {
            if (!await roleManager.RoleExistsAsync(name))
                await roleManager.CreateAsync(new ApplicationRole(name, responsibility));
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            (Email: "requestor@test.com", FirstName: "Alice",  LastName: "Johnson",  Role: UserRole.Requestor),
            (Email: "manager@test.com",   FirstName: "Bob",    LastName: "Williams", Role: UserRole.Manager),
            (Email: "finance@test.com",   FirstName: "Carol",  LastName: "Brown",    Role: UserRole.FinanceAdmin),
            (Email: "admin@test.com",     FirstName: "Dave",   LastName: "Taylor",   Role: UserRole.SystemAdmin),
        };

        var seedPassword = configuration["Seeding:DefaultPassword"] ?? "Test@123!";

        foreach (var (email, firstName, lastName, role) in users)
        {
            var existing = await userManager.FindByEmailAsync(email);

            if (existing != null)
            {
                // Ensure profile fields and password are always in sync with seed config
                existing.FirstName = firstName;
                existing.LastName = lastName;
                existing.EmailConfirmed = true;
                await userManager.UpdateAsync(existing);

                var resetToken = await userManager.GeneratePasswordResetTokenAsync(existing);
                await userManager.ResetPasswordAsync(existing, resetToken, seedPassword);

                if (!await userManager.IsInRoleAsync(existing, role))
                    await userManager.AddToRoleAsync(existing, role);

                continue;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, seedPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
            else
                logger.LogWarning("Failed to create seed user {Email}: {Errors}",
                    email, string.Join(", ", result.Errors.Select(e => e.Description)));
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

using Microsoft.AspNetCore.Identity;

namespace SponsorshipWorkflow.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole() { }

    public ApplicationRole(string name, string responsibility) : base(name)
    {
        Responsibility = responsibility;
    }

    public string Responsibility { get; set; } = string.Empty;
}

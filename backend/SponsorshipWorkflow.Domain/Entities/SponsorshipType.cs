using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Domain.Entities;

public class SponsorshipType : BaseEntity
{
    private SponsorshipType() { }

    public SponsorshipType(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public ICollection<SponsorshipRequest> SponsorshipRequests { get; set; } = new List<SponsorshipRequest>();

    public void Update(string name, bool isActive)
    {
        Name = name;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}

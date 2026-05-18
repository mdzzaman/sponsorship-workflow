using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Domain.Entities;

public class SponsorshipType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<SponsorshipRequest> SponsorshipRequests { get; set; } = new List<SponsorshipRequest>();
}

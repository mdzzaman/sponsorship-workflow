using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Domain.Entities;

public class WorkflowHistory : BaseEntity
{
    public Guid RequestId { get; set; }
    public RequestStatus FromStatus { get; set; }
    public RequestStatus ToStatus { get; set; }
    public string ActorId { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime ActionedAt { get; set; } = DateTime.UtcNow;

    public SponsorshipRequest Request { get; set; } = null!;
}

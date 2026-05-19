using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Domain.Entities;

public class WorkflowHistory : BaseEntity
{
    private WorkflowHistory() { }

    public WorkflowHistory(Guid requestId, RequestStatus fromStatus, RequestStatus toStatus,
        string actorId, string actorName, string? remarks, DateTime recordedAt)
    {
        RequestId = requestId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ActorId = actorId;
        ActorName = actorName;
        Remarks = remarks;
        RecordedAt = recordedAt;
    }

    public Guid RequestId { get; private set; }
    public RequestStatus FromStatus { get; private set; }
    public RequestStatus ToStatus { get; private set; }
    public string ActorId { get; private set; } = string.Empty;
    public string ActorName { get; private set; } = string.Empty;
    public string? Remarks { get; private set; }
    public DateTime RecordedAt { get; private set; }

    public SponsorshipRequest Request { get; private set; } = null!;
}

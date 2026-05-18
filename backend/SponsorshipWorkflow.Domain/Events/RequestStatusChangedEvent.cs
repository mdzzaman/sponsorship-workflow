using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Domain.Events;

public record RequestStatusChangedEvent(
    Guid RequestId,
    RequestStatus FromStatus,
    RequestStatus ToStatus,
    string ActorId,
    string ActorName,
    string? Remarks
) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

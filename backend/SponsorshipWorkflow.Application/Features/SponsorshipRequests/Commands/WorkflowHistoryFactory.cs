using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Events;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

/// <summary>
/// Creates a WorkflowHistory record from the domain event raised by SponsorshipRequest.ChangeStatus().
/// All workflow command handlers must call ChangeStatus() before using this factory,
/// and must call entity.ClearDomainEvents() after adding the history record.
/// </summary>
internal static class WorkflowHistoryFactory
{
    internal static WorkflowHistory FromLastEvent(SponsorshipRequest entity)
    {
        var evt = entity.DomainEvents.OfType<RequestStatusChangedEvent>().Last();
        return new WorkflowHistory
        {
            RequestId = evt.RequestId,
            FromStatus = evt.FromStatus,
            ToStatus = evt.ToStatus,
            ActorId = evt.ActorId,
            ActorName = evt.ActorName,
            Remarks = evt.Remarks,
            ActionedAt = evt.OccurredAt
        };
    }
}

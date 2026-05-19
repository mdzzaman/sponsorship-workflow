using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Events;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

internal static class WorkflowHistoryFactory
{
    internal static WorkflowHistory FromLastEvent(SponsorshipRequest entity)
    {
        var evt = entity.DomainEvents.OfType<RequestStatusChangedEvent>().LastOrDefault()
            ?? throw new InvalidOperationException($"No RequestStatusChangedEvent found on request {entity.Id}.");

        return new WorkflowHistory(evt.RequestId, evt.FromStatus, evt.ToStatus,
            evt.ActorId, evt.ActorName, evt.Remarks, evt.OccurredAt);
    }
}

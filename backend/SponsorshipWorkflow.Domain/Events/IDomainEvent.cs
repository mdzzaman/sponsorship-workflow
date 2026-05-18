namespace SponsorshipWorkflow.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}

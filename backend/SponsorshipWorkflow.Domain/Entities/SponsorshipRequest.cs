using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Domain.Enums;
using SponsorshipWorkflow.Domain.Events;

namespace SponsorshipWorkflow.Domain.Entities;

public class SponsorshipRequest : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string RequestorId { get; set; } = string.Empty;
    public string RequestorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public Guid SponsorshipTypeId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Justification { get; set; } = string.Empty;
    public string? ExpectedBenefit { get; set; }
    public string? Remarks { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Draft;

    public SponsorshipType SponsorshipType { get; set; } = null!;
    public ICollection<WorkflowHistory> WorkflowHistories { get; set; } = new List<WorkflowHistory>();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void ChangeStatus(RequestStatus newStatus, string actorId, string actorName, string? remarks = null)
    {
        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new RequestStatusChangedEvent(
            Id, oldStatus, newStatus, actorId, actorName, remarks));
    }
}

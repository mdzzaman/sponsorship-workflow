using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Domain.Enums;
using SponsorshipWorkflow.Domain.Events;

namespace SponsorshipWorkflow.Domain.Entities;

public class SponsorshipRequest : BaseEntity
{
    private SponsorshipRequest() { }

    public static SponsorshipRequest Create(
        string title, string requestorId, string requestorName, string department,
        Guid sponsorshipTypeId, string eventName, DateTime eventDate, decimal requestedAmount,
        string justification, string? expectedBenefit, string? remarks)
    {
        return new SponsorshipRequest
        {
            Title = title,
            RequestorId = requestorId,
            RequestorName = requestorName,
            Department = department,
            SponsorshipTypeId = sponsorshipTypeId,
            EventName = eventName,
            EventDate = eventDate,
            RequestedAmount = requestedAmount,
            Justification = justification,
            ExpectedBenefit = expectedBenefit,
            Remarks = remarks
        };
    }

    public string Title { get; private set; } = string.Empty;
    public string RequestorId { get; private set; } = string.Empty;
    public string RequestorName { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public Guid SponsorshipTypeId { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public DateTime EventDate { get; private set; }
    public decimal RequestedAmount { get; private set; }
    public string Justification { get; private set; } = string.Empty;
    public string? ExpectedBenefit { get; private set; }
    public string? Remarks { get; private set; }
    public RequestStatus Status { get; private set; } = RequestStatus.Draft;

    public SponsorshipType SponsorshipType { get; private set; } = null!;
    public ICollection<WorkflowHistory> WorkflowHistories { get; private set; } = new List<WorkflowHistory>();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    private bool IsOwner(string requestorId) => RequestorId == requestorId;

    public Result Update(string requestorId, string title, string department, Guid sponsorshipTypeId,
        string eventName, DateTime eventDate, decimal requestedAmount, string justification,
        string? expectedBenefit, string? remarks)
    {
        if (!IsOwner(requestorId))
            return Result.Failure("You can only edit your own requests.");
        if (Status != RequestStatus.Draft)
            return Result.Failure("Only draft requests can be edited.");

        Title = title;
        Department = department;
        SponsorshipTypeId = sponsorshipTypeId;
        EventName = eventName;
        EventDate = eventDate;
        RequestedAmount = requestedAmount;
        Justification = justification;
        ExpectedBenefit = expectedBenefit;
        Remarks = remarks;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Submit(string requestorId, string requestorName)
    {
        if (!IsOwner(requestorId))
            return Result.Failure("You can only submit your own requests.");
        if (Status != RequestStatus.Draft)
            return Result.Failure("Only draft requests can be submitted.");

        ChangeStatus(RequestStatus.PendingManagerApproval, requestorId, requestorName, "Submitted for approval");
        return Result.Success();
    }

    public Result Cancel(string requestorId, string requestorName)
    {
        if (!IsOwner(requestorId))
            return Result.Failure("You can only cancel your own requests.");

        RequestStatus[] cancellable = [RequestStatus.Draft, RequestStatus.PendingManagerApproval, RequestStatus.PendingFinanceReview];
        if (!cancellable.Contains(Status))
            return Result.Failure("This request cannot be cancelled at its current status.");

        ChangeStatus(RequestStatus.Cancelled, requestorId, requestorName, "Cancelled by requestor");
        return Result.Success();
    }

    public Result Approve(string actorId, string actorName, IReadOnlyList<string> actorRoles, string? remarks)
    {
        if (Status == RequestStatus.PendingManagerApproval && actorRoles.Contains(UserRole.Manager))
            return Transition(RequestStatus.PendingFinanceReview, actorId, actorName, remarks);

        if (Status == RequestStatus.PendingFinanceReview && actorRoles.Contains(UserRole.FinanceAdmin))
            return Transition(RequestStatus.Approved, actorId, actorName, remarks);

        return Result.Failure("You are not authorized to approve this request at its current status.");
    }

    public Result Reject(string actorId, string actorName, IReadOnlyList<string> actorRoles, string? remarks)
    {
        if (string.IsNullOrWhiteSpace(remarks))
            return Result.Failure("A reason is required when rejecting a request.");

        if (Status == RequestStatus.PendingManagerApproval && actorRoles.Contains(UserRole.Manager))
            return Transition(RequestStatus.Rejected, actorId, actorName, remarks);

        if (Status == RequestStatus.PendingFinanceReview && actorRoles.Contains(UserRole.FinanceAdmin))
            return Transition(RequestStatus.Rejected, actorId, actorName, remarks);

        return Result.Failure("You are not authorized to reject this request at its current status.");
    }

    private Result Transition(RequestStatus next, string actorId, string actorName, string? remarks)
    {
        ChangeStatus(next, actorId, actorName, remarks);
        return Result.Success();
    }

    private void ChangeStatus(RequestStatus newStatus, string actorId, string actorName, string? remarks = null)
    {
        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(new RequestStatusChangedEvent(
            Id, oldStatus, newStatus, actorId, actorName, remarks));
    }
}

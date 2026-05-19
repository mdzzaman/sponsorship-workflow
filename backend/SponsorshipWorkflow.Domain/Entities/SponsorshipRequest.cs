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

    public Result ApproveByManager(string managerId, string managerName, string? remarks)
    {
        if (Status != RequestStatus.PendingManagerApproval)
            return Result.Failure("Request is not pending manager approval.");

        ChangeStatus(RequestStatus.PendingFinanceReview, managerId, managerName, remarks);
        return Result.Success();
    }

    public Result RejectByManager(string managerId, string managerName, string remarks)
    {
        if (Status != RequestStatus.PendingManagerApproval)
            return Result.Failure("Request is not pending manager approval.");

        ChangeStatus(RequestStatus.Rejected, managerId, managerName, remarks);
        return Result.Success();
    }

    public Result ApproveByFinance(string financeId, string financeName, string? remarks)
    {
        if (Status != RequestStatus.PendingFinanceReview)
            return Result.Failure("Request is not pending finance review.");

        ChangeStatus(RequestStatus.Approved, financeId, financeName, remarks);
        return Result.Success();
    }

    public Result RejectByFinance(string financeId, string financeName, string remarks)
    {
        if (Status != RequestStatus.PendingFinanceReview)
            return Result.Failure("Request is not pending finance review.");

        ChangeStatus(RequestStatus.Rejected, financeId, financeName, remarks);
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

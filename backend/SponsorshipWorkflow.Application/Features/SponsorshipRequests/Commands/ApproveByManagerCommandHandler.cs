using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public class ApproveByManagerCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ApproveByManagerCommand, Result>
{
    public async Task<Result> Handle(ApproveByManagerCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");
        if (entity.Status != RequestStatus.PendingManagerApproval)
            return Result.Failure("Request is not pending manager approval.");

        entity.ChangeStatus(RequestStatus.PendingFinanceReview, request.ManagerId, request.ManagerName, request.Remarks);
        db.WorkflowHistories.Add(CreateHistory(entity));
        entity.ClearDomainEvents();

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static WorkflowHistory CreateHistory(SponsorshipRequest entity)
    {
        var evt = entity.DomainEvents.OfType<Domain.Events.RequestStatusChangedEvent>().Last();
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

public class RejectByManagerCommandHandler(IApplicationDbContext db)
    : IRequestHandler<RejectByManagerCommand, Result>
{
    public async Task<Result> Handle(RejectByManagerCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");
        if (entity.Status != RequestStatus.PendingManagerApproval)
            return Result.Failure("Request is not pending manager approval.");

        entity.ChangeStatus(RequestStatus.Rejected, request.ManagerId, request.ManagerName, request.Remarks);
        db.WorkflowHistories.Add(new WorkflowHistory
        {
            RequestId = request.RequestId,
            FromStatus = RequestStatus.PendingManagerApproval,
            ToStatus = RequestStatus.Rejected,
            ActorId = request.ManagerId,
            ActorName = request.ManagerName,
            Remarks = request.Remarks,
            ActionedAt = DateTime.UtcNow
        });
        entity.ClearDomainEvents();

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

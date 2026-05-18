using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public class ApproveByFinanceCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ApproveByFinanceCommand, Result>
{
    public async Task<Result> Handle(ApproveByFinanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");
        if (entity.Status != RequestStatus.PendingFinanceReview)
            return Result.Failure("Request is not pending finance review.");

        entity.ChangeStatus(RequestStatus.Approved, request.FinanceId, request.FinanceName, request.Remarks);
        db.WorkflowHistories.Add(new WorkflowHistory
        {
            RequestId = request.RequestId,
            FromStatus = RequestStatus.PendingFinanceReview,
            ToStatus = RequestStatus.Approved,
            ActorId = request.FinanceId,
            ActorName = request.FinanceName,
            Remarks = request.Remarks,
            ActionedAt = DateTime.UtcNow
        });
        entity.ClearDomainEvents();

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class RejectByFinanceCommandHandler(IApplicationDbContext db)
    : IRequestHandler<RejectByFinanceCommand, Result>
{
    public async Task<Result> Handle(RejectByFinanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");
        if (entity.Status != RequestStatus.PendingFinanceReview)
            return Result.Failure("Request is not pending finance review.");

        entity.ChangeStatus(RequestStatus.Rejected, request.FinanceId, request.FinanceName, request.Remarks);
        db.WorkflowHistories.Add(new WorkflowHistory
        {
            RequestId = request.RequestId,
            FromStatus = RequestStatus.PendingFinanceReview,
            ToStatus = RequestStatus.Rejected,
            ActorId = request.FinanceId,
            ActorName = request.FinanceName,
            Remarks = request.Remarks,
            ActionedAt = DateTime.UtcNow
        });
        entity.ClearDomainEvents();

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

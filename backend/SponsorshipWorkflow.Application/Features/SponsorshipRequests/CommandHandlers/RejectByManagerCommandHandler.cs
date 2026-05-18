using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

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
        db.WorkflowHistories.Add(WorkflowHistoryFactory.FromLastEvent(entity));
        entity.ClearDomainEvents();

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure("This request was modified by another user. Please refresh and try again.");
        }

        return Result.Success();
    }
}

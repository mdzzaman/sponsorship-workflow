using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public class SubmitRequestCommandHandler(IApplicationDbContext db)
    : IRequestHandler<SubmitRequestCommand, Result>
{
    public async Task<Result> Handle(SubmitRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null)
            return Result.Failure("Request not found.");

        if (entity.RequestorId != request.RequestorId)
            return Result.Failure("You can only submit your own requests.");

        if (entity.Status != RequestStatus.Draft)
            return Result.Failure("Only draft requests can be submitted.");

        entity.ChangeStatus(RequestStatus.PendingManagerApproval, request.RequestorId, request.RequestorName, "Submitted for approval");

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

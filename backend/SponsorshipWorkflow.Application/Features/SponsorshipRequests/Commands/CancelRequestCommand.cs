using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record CancelRequestCommand(Guid RequestId, string RequestorId, string RequestorName)
    : IRequest<Result>;

public class CancelRequestCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CancelRequestCommand, Result>
{
    private static readonly RequestStatus[] CancellableStatuses =
    [
        RequestStatus.Draft,
        RequestStatus.PendingManagerApproval,
        RequestStatus.PendingFinanceReview
    ];

    public async Task<Result> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");
        if (entity.RequestorId != request.RequestorId)
            return Result.Failure("You can only cancel your own requests.");
        if (!CancellableStatuses.Contains(entity.Status))
            return Result.Failure("This request cannot be cancelled at its current status.");

        var fromStatus = entity.Status;
        entity.ChangeStatus(RequestStatus.Cancelled, request.RequestorId, request.RequestorName, "Cancelled by requestor");
        db.WorkflowHistories.Add(new WorkflowHistory
        {
            RequestId = request.RequestId,
            FromStatus = fromStatus,
            ToStatus = RequestStatus.Cancelled,
            ActorId = request.RequestorId,
            ActorName = request.RequestorName,
            Remarks = "Cancelled by requestor",
            ActionedAt = DateTime.UtcNow
        });
        entity.ClearDomainEvents();

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

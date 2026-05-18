using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

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

        entity.ChangeStatus(RequestStatus.Cancelled, request.RequestorId, request.RequestorName, "Cancelled by requestor");
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

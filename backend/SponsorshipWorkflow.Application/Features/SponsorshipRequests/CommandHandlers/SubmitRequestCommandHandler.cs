using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

public class SubmitRequestCommandHandler(IApplicationDbContext db)
    : IRequestHandler<SubmitRequestCommand, Result>
{
    public async Task<Result> Handle(SubmitRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");
        if (entity.RequestorId != request.RequestorId) return Result.Failure("You can only submit your own requests.");
        if (entity.Status != RequestStatus.Draft) return Result.Failure("Only draft requests can be submitted.");

        entity.ChangeStatus(RequestStatus.PendingManagerApproval, request.RequestorId, request.RequestorName, "Submitted for approval");
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

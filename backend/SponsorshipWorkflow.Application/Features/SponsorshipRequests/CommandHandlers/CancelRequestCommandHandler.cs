using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

public class CancelRequestCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CancelRequestCommand, Result>
{
    public async Task<Result> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");

        var result = entity.Cancel(request.RequestorId, request.RequestorName);
        if (!result.IsSuccess) return result;

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

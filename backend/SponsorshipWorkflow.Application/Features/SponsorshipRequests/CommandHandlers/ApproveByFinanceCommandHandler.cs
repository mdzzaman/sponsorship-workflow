using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

public class ApproveByFinanceCommandHandler(IApplicationDbContext db)
    : IRequestHandler<ApproveByFinanceCommand, Result>
{
    public async Task<Result> Handle(ApproveByFinanceCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result.Failure("Request not found.");

        var result = entity.ApproveByFinance(request.FinanceId, request.FinanceName, request.Remarks);
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

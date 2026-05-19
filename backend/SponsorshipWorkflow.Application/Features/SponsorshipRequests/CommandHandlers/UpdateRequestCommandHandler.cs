using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

public class UpdateRequestCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateRequestCommand, Result<SponsorshipRequestResponse>>
{
    public async Task<Result<SponsorshipRequestResponse>> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result<SponsorshipRequestResponse>.Failure("Request not found.");

        var typeExists = await db.SponsorshipTypes.FindAsync([request.SponsorshipTypeId], cancellationToken);
        if (typeExists == null || !typeExists.IsActive)
            return Result<SponsorshipRequestResponse>.Failure("Invalid sponsorship type.");

        var result = entity.Update(request.RequestorId, request.Title, request.Department,
            request.SponsorshipTypeId, request.EventName, request.EventDate,
            request.RequestedAmount, request.Justification, request.ExpectedBenefit, request.Remarks);

        if (!result.IsSuccess) return Result<SponsorshipRequestResponse>.Failure(result.Error!);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<SponsorshipRequestResponse>.Failure("This request was modified by another user. Please refresh and try again.");
        }

        return Result<SponsorshipRequestResponse>.Success(entity.ToResponse());
    }
}

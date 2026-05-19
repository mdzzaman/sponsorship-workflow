using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

public class UpdateRequestCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateRequestCommand, Result<SponsorshipRequestDto>>
{
    public async Task<Result<SponsorshipRequestDto>> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return Result<SponsorshipRequestDto>.Failure("Request not found.");

        var typeExists = await db.SponsorshipTypes.FindAsync([request.SponsorshipTypeId], cancellationToken);
        if (typeExists == null || !typeExists.IsActive)
            return Result<SponsorshipRequestDto>.Failure("Invalid sponsorship type.");

        var result = entity.Update(request.RequestorId, request.Title, request.Department,
            request.SponsorshipTypeId, request.EventName, request.EventDate,
            request.RequestedAmount, request.Justification, request.ExpectedBenefit, request.Remarks);

        if (!result.IsSuccess) return Result<SponsorshipRequestDto>.Failure(result.Error!);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<SponsorshipRequestDto>.Failure("This request was modified by another user. Please refresh and try again.");
        }

        return Result<SponsorshipRequestDto>.Success(entity.ToDto());
    }
}

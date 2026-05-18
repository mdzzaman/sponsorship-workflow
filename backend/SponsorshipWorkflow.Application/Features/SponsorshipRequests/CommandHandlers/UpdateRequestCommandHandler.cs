using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

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
        if (entity.RequestorId != request.RequestorId)
            return Result<SponsorshipRequestDto>.Failure("You can only edit your own requests.");
        if (entity.Status != RequestStatus.Draft)
            return Result<SponsorshipRequestDto>.Failure("Only draft requests can be edited.");

        var typeExists = await db.SponsorshipTypes.FindAsync([request.SponsorshipTypeId], cancellationToken);
        if (typeExists == null || !typeExists.IsActive)
            return Result<SponsorshipRequestDto>.Failure("Invalid sponsorship type.");

        entity.Title = request.Title;
        entity.Department = request.Department;
        entity.SponsorshipTypeId = request.SponsorshipTypeId;
        entity.EventName = request.EventName;
        entity.EventDate = request.EventDate;
        entity.RequestedAmount = request.RequestedAmount;
        entity.Justification = request.Justification;
        entity.ExpectedBenefit = request.ExpectedBenefit;
        entity.Remarks = request.Remarks;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return Result<SponsorshipRequestDto>.Success(entity.ToDto());
    }
}

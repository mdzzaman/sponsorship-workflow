using MediatR;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandHandlers;

public class CreateRequestCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateRequestCommand, Result<SponsorshipRequestDto>>
{
    public async Task<Result<SponsorshipRequestDto>> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var typeExists = await db.SponsorshipTypes.FindAsync([request.SponsorshipTypeId], cancellationToken);
        if (typeExists == null || !typeExists.IsActive)
            return Result<SponsorshipRequestDto>.Failure("Invalid or inactive sponsorship type.");

        var entity = new SponsorshipRequest
        {
            Title = request.Title,
            RequestorId = request.RequestorId,
            RequestorName = request.RequestorName,
            Department = request.Department,
            SponsorshipTypeId = request.SponsorshipTypeId,
            EventName = request.EventName,
            EventDate = request.EventDate,
            RequestedAmount = request.RequestedAmount,
            Justification = request.Justification,
            ExpectedBenefit = request.ExpectedBenefit,
            Remarks = request.Remarks,
            Status = RequestStatus.Draft
        };

        db.SponsorshipRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Result<SponsorshipRequestDto>.Success(entity.ToDto());
    }
}

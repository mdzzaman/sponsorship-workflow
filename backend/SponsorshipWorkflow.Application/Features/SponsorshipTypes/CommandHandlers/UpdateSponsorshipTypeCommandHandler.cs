using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.CommandHandlers;

public class UpdateSponsorshipTypeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateSponsorshipTypeCommand, Result<SponsorshipTypeDto>>
{
    public async Task<Result<SponsorshipTypeDto>> Handle(UpdateSponsorshipTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipTypes.FindAsync([request.Id], cancellationToken);
        if (entity == null) return Result<SponsorshipTypeDto>.Failure("Sponsorship type not found.");

        entity.Update(request.Name, request.IsActive);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<SponsorshipTypeDto>.Failure("This sponsorship type was modified by another user. Please refresh and try again.");
        }

        return Result<SponsorshipTypeDto>.Success(new SponsorshipTypeDto { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive });
    }
}

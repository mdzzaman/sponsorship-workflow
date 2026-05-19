using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.CommandHandlers;

public class UpdateSponsorshipTypeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateSponsorshipTypeCommand, Result<SponsorshipTypeResponse>>
{
    public async Task<Result<SponsorshipTypeResponse>> Handle(UpdateSponsorshipTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipTypes.FindAsync([request.Id], cancellationToken);
        if (entity == null) return Result<SponsorshipTypeResponse>.Failure("Sponsorship type not found.");

        entity.Update(request.Name, request.IsActive);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<SponsorshipTypeResponse>.Failure("This sponsorship type was modified by another user. Please refresh and try again.");
        }

        return Result<SponsorshipTypeResponse>.Success(entity.ToResponse());
    }
}

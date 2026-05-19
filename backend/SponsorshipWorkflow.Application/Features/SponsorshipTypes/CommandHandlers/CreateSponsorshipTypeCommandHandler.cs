using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.CommandHandlers;

public class CreateSponsorshipTypeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateSponsorshipTypeCommand, Result<SponsorshipTypeResponse>>
{
    public async Task<Result<SponsorshipTypeResponse>> Handle(CreateSponsorshipTypeCommand request, CancellationToken cancellationToken)
    {
        if (await db.SponsorshipTypes.AnyAsync(t => t.Name == request.Name, cancellationToken))
            return Result<SponsorshipTypeResponse>.Failure("A sponsorship type with this name already exists.");

        var entity = new SponsorshipType(request.Name);
        db.SponsorshipTypes.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Result<SponsorshipTypeResponse>.Success(entity.ToResponse());
    }
}

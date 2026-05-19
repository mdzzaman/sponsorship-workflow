using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Domain.Common;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.CommandHandlers;

public class CreateSponsorshipTypeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateSponsorshipTypeCommand, Result<SponsorshipTypeDto>>
{
    public async Task<Result<SponsorshipTypeDto>> Handle(CreateSponsorshipTypeCommand request, CancellationToken cancellationToken)
    {
        if (await db.SponsorshipTypes.AnyAsync(t => t.Name == request.Name, cancellationToken))
            return Result<SponsorshipTypeDto>.Failure("A sponsorship type with this name already exists.");

        var entity = new SponsorshipType { Name = request.Name };
        db.SponsorshipTypes.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Result<SponsorshipTypeDto>.Success(new SponsorshipTypeDto { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive });
    }
}

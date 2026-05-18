using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;

public record CreateSponsorshipTypeCommand(string Name) : IRequest<Result<SponsorshipTypeDto>>;
public record UpdateSponsorshipTypeCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<SponsorshipTypeDto>>;

public class CreateSponsorshipTypeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateSponsorshipTypeCommand, Result<SponsorshipTypeDto>>
{
    public async Task<Result<SponsorshipTypeDto>> Handle(CreateSponsorshipTypeCommand request, CancellationToken cancellationToken)
    {
        if (await db.SponsorshipTypes.AnyAsync(t => t.Name == request.Name, cancellationToken))
            return Result<SponsorshipTypeDto>.Failure("Sponsorship type with this name already exists.");

        var entity = new SponsorshipType { Name = request.Name };
        db.SponsorshipTypes.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Result<SponsorshipTypeDto>.Success(new SponsorshipTypeDto { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive });
    }
}

public class UpdateSponsorshipTypeCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateSponsorshipTypeCommand, Result<SponsorshipTypeDto>>
{
    public async Task<Result<SponsorshipTypeDto>> Handle(UpdateSponsorshipTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipTypes.FindAsync([request.Id], cancellationToken);
        if (entity == null) return Result<SponsorshipTypeDto>.Failure("Sponsorship type not found.");

        entity.Name = request.Name;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return Result<SponsorshipTypeDto>.Success(new SponsorshipTypeDto { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive });
    }
}

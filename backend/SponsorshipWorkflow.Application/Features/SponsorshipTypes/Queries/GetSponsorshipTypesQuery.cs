using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.Queries;

public record GetSponsorshipTypesQuery(bool ActiveOnly = true) : IRequest<List<SponsorshipTypeDto>>;

public class GetSponsorshipTypesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetSponsorshipTypesQuery, List<SponsorshipTypeDto>>
{
    public async Task<List<SponsorshipTypeDto>> Handle(GetSponsorshipTypesQuery request, CancellationToken cancellationToken)
    {
        var query = db.SponsorshipTypes.AsQueryable();
        if (request.ActiveOnly) query = query.Where(t => t.IsActive);

        return await query
            .OrderBy(t => t.Name)
            .Select(t => new SponsorshipTypeDto { Id = t.Id, Name = t.Name, IsActive = t.IsActive })
            .ToListAsync(cancellationToken);
    }
}

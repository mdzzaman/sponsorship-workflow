using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Queries;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.QueryHandlers;

public class GetSponsorshipTypesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetSponsorshipTypesQuery, List<SponsorshipTypeResponse>>
{
    public async Task<List<SponsorshipTypeResponse>> Handle(GetSponsorshipTypesQuery request, CancellationToken cancellationToken)
    {
        var query = db.SponsorshipTypes.AsNoTracking().AsQueryable();
        if (request.ActiveOnly) query = query.Where(t => t.IsActive);

        var types = await query
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        return types.ConvertAll(t => t.ToResponse());
    }
}

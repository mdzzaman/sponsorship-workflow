using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetAllRequestsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllRequestsQuery, List<SponsorshipRequestResponse>>
{
    public async Task<List<SponsorshipRequestResponse>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        var entities = await db.SponsorshipRequests
            .AsNoTracking()
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.ConvertAll(r => r.ToResponse());
    }
}

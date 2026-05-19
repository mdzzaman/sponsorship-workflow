using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetMyRequestsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetMyRequestsQuery, List<SponsorshipRequestResponse>>
{
    public async Task<List<SponsorshipRequestResponse>> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        var entities = await db.SponsorshipRequests
            .AsNoTracking()
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .Where(r => r.RequestorId == request.RequestorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.ConvertAll(r => r.ToResponse());
    }
}

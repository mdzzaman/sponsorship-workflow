using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetMyRequestsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetMyRequestsQuery, List<SponsorshipRequestResponse>>
{
    public async Task<List<SponsorshipRequestResponse>> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        return await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .Where(r => r.RequestorId == request.RequestorId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToResponse())
            .ToListAsync(cancellationToken);
    }
}

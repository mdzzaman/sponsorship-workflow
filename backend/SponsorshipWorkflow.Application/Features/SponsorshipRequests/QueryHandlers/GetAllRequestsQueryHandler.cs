using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetAllRequestsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllRequestsQuery, List<SponsorshipRequestResponse>>
{
    public async Task<List<SponsorshipRequestResponse>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        return await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToResponse())
            .ToListAsync(cancellationToken);
    }
}

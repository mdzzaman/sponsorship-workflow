using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetPendingManagerApprovalsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPendingManagerApprovalsQuery, List<SponsorshipRequestResponse>>
{
    public async Task<List<SponsorshipRequestResponse>> Handle(GetPendingManagerApprovalsQuery request, CancellationToken cancellationToken)
    {
        return await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .Where(r => r.Status == RequestStatus.PendingManagerApproval)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToResponse())
            .ToListAsync(cancellationToken);
    }
}

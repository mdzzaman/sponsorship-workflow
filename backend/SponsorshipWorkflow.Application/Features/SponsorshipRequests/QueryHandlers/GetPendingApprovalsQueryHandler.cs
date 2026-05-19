using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetPendingApprovalsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPendingApprovalsQuery, List<SponsorshipRequestResponse>>
{
    public async Task<List<SponsorshipRequestResponse>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var statuses = new List<RequestStatus>();

        if (request.ActorRoles.Contains(UserRole.Manager))
            statuses.Add(RequestStatus.PendingManagerApproval);

        if (request.ActorRoles.Contains(UserRole.FinanceAdmin))
            statuses.Add(RequestStatus.PendingFinanceReview);

        return await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .Where(r => statuses.Contains(r.Status))
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToResponse())
            .ToListAsync(cancellationToken);
    }
}

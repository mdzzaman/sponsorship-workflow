using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common.Mappings;
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

        var entities = await db.SponsorshipRequests
            .AsNoTracking()
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .Where(r => statuses.Contains(r.Status))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.ConvertAll(r => r.ToResponse());
    }
}

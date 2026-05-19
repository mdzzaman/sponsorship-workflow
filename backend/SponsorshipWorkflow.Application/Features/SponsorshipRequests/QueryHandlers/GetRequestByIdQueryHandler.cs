using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetRequestByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetRequestByIdQuery, SponsorshipRequestResponse?>
{
    public async Task<SponsorshipRequestResponse?> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .AsNoTracking()
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null) return null;

        var canView = entity.RequestorId == request.ActorId
            || request.ActorRoles.Contains(UserRole.SystemAdmin)
            || request.ActorRoles.Contains(UserRole.Manager)
            || request.ActorRoles.Contains(UserRole.FinanceAdmin);

        return canView ? entity.ToResponse() : null;
    }
}

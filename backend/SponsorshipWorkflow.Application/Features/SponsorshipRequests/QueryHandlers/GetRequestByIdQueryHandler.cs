using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetRequestByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetRequestByIdQuery, SponsorshipRequestResponse?>
{
    public async Task<SponsorshipRequestResponse?> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        return entity?.ToResponse();
    }
}

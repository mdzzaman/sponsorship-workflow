using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetRequestByIdQuery(Guid RequestId) : IRequest<SponsorshipRequestDto?>;

public class GetRequestByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetRequestByIdQuery, SponsorshipRequestDto?>
{
    public async Task<SponsorshipRequestDto?> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        return entity?.ToDto();
    }
}

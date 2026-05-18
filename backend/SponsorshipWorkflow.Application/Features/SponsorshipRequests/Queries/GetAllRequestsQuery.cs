using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Interfaces;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetAllRequestsQuery : IRequest<List<SponsorshipRequestDto>>;

public class GetAllRequestsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllRequestsQuery, List<SponsorshipRequestDto>>
{
    public async Task<List<SponsorshipRequestDto>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        return await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToDto())
            .ToListAsync(cancellationToken);
    }
}

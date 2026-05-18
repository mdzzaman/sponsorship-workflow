using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetPendingManagerApprovalsQuery : IRequest<List<SponsorshipRequestDto>>;

public class GetPendingManagerApprovalsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPendingManagerApprovalsQuery, List<SponsorshipRequestDto>>
{
    public async Task<List<SponsorshipRequestDto>> Handle(GetPendingManagerApprovalsQuery request, CancellationToken cancellationToken)
    {
        return await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .Where(r => r.Status == RequestStatus.PendingManagerApproval)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToDto())
            .ToListAsync(cancellationToken);
    }
}

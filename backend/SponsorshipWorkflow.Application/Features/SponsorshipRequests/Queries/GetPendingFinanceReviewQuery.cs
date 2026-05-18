using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.DTOs;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Enums;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetPendingFinanceReviewQuery : IRequest<List<SponsorshipRequestDto>>;

public class GetPendingFinanceReviewQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPendingFinanceReviewQuery, List<SponsorshipRequestDto>>
{
    public async Task<List<SponsorshipRequestDto>> Handle(GetPendingFinanceReviewQuery request, CancellationToken cancellationToken)
    {
        return await db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistories)
            .Where(r => r.Status == RequestStatus.PendingFinanceReview)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => r.ToDto())
            .ToListAsync(cancellationToken);
    }
}

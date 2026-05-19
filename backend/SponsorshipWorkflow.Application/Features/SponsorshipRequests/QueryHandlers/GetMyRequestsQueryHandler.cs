using MediatR;
using Microsoft.EntityFrameworkCore;
using SponsorshipWorkflow.Application.Common.Mappings;
using SponsorshipWorkflow.Application.Responses;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.QueryHandlers;

public class GetMyRequestsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetMyRequestsQuery, PagedResult<SponsorshipRequestResponse>>
{
    public async Task<PagedResult<SponsorshipRequestResponse>> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = db.SponsorshipRequests
            .AsNoTracking()
            .Include(r => r.SponsorshipType)
            .Where(r => r.RequestorId == request.RequestorId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim().ToLower();
            query = query.Where(r =>
                r.Title.ToLower().Contains(s) ||
                r.EventName.ToLower().Contains(s) ||
                r.Department.ToLower().Contains(s));
        }

        query = ApplySort(query, request.SortBy, request.SortDesc);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(r => r.WorkflowHistories)
            .ToListAsync(cancellationToken);

        return new PagedResult<SponsorshipRequestResponse>
        {
            Items = items.ConvertAll(r => r.ToResponse()),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static IQueryable<SponsorshipRequest> ApplySort(IQueryable<SponsorshipRequest> q, string? sortBy, bool desc) =>
        (sortBy?.ToLower(), desc) switch
        {
            ("title",           false) => q.OrderBy(r => r.Title),
            ("title",           true)  => q.OrderByDescending(r => r.Title),
            ("eventname",       false) => q.OrderBy(r => r.EventName),
            ("eventname",       true)  => q.OrderByDescending(r => r.EventName),
            ("requestedamount", false) => q.OrderBy(r => r.RequestedAmount),
            ("requestedamount", true)  => q.OrderByDescending(r => r.RequestedAmount),
            ("status",          false) => q.OrderBy(r => r.Status),
            ("status",          true)  => q.OrderByDescending(r => r.Status),
            ("createdat",       false) => q.OrderBy(r => r.CreatedAt),
            _                          => q.OrderByDescending(r => r.CreatedAt),
        };
}

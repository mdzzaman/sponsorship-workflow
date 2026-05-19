using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetPendingApprovalsQuery(
    IReadOnlyList<string> ActorRoles,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    bool SortDesc = true,
    string? Search = null) : IRequest<PagedResult<SponsorshipRequestResponse>>;

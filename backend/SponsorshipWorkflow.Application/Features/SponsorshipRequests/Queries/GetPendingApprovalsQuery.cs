using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetPendingApprovalsQuery(IReadOnlyList<string> ActorRoles)
    : IRequest<List<SponsorshipRequestResponse>>;

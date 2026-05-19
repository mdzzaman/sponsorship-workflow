using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetRequestByIdQuery(Guid RequestId, string ActorId, IReadOnlyList<string> ActorRoles)
    : IRequest<SponsorshipRequestResponse?>;

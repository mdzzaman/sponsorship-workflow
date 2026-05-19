using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetPendingManagerApprovalsQuery : IRequest<List<SponsorshipRequestResponse>>;

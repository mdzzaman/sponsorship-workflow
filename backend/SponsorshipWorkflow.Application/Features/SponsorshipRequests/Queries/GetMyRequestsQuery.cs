using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetMyRequestsQuery(string RequestorId) : IRequest<List<SponsorshipRequestResponse>>;

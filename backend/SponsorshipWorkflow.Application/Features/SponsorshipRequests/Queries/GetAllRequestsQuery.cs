using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetAllRequestsQuery : IRequest<List<SponsorshipRequestResponse>>;

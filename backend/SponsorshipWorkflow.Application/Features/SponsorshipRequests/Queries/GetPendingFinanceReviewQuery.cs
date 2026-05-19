using MediatR;
using SponsorshipWorkflow.Application.Responses;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetPendingFinanceReviewQuery : IRequest<List<SponsorshipRequestResponse>>;

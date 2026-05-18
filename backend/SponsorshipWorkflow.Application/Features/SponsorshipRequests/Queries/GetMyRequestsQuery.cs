using MediatR;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetMyRequestsQuery(string RequestorId) : IRequest<List<SponsorshipRequestDto>>;

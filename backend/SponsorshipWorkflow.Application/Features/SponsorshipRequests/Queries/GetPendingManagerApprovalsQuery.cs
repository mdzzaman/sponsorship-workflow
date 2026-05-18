using MediatR;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetPendingManagerApprovalsQuery : IRequest<List<SponsorshipRequestDto>>;

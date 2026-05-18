using MediatR;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Queries;

public record GetRequestByIdQuery(Guid RequestId) : IRequest<SponsorshipRequestDto?>;

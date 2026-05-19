using MediatR;
using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record CancelRequestCommand(Guid RequestId, string RequestorId, string RequestorName)
    : IRequest<Result>;

using MediatR;
using SponsorshipWorkflow.Application.Common;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record SubmitRequestCommand(Guid RequestId, string RequestorId, string RequestorName)
    : IRequest<Result>;

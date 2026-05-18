using MediatR;
using SponsorshipWorkflow.Application.Common;
using SponsorshipWorkflow.Application.DTOs;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record UpdateRequestCommand(
    Guid RequestId,
    string RequestorId,
    string Title,
    string Department,
    Guid SponsorshipTypeId,
    string EventName,
    DateTime EventDate,
    decimal RequestedAmount,
    string Justification,
    string? ExpectedBenefit,
    string? Remarks
) : IRequest<Result<SponsorshipRequestDto>>;

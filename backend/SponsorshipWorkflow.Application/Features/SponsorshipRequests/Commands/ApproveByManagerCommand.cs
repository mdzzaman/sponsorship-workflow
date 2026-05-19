using MediatR;
using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record ApproveByManagerCommand(Guid RequestId, string ManagerId, string ManagerName, string? Remarks)
    : IRequest<Result>;

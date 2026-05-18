using MediatR;
using SponsorshipWorkflow.Application.Common;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record RejectByManagerCommand(Guid RequestId, string ManagerId, string ManagerName, string Remarks)
    : IRequest<Result>;

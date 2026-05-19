using MediatR;
using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record RejectRequestCommand(Guid RequestId, string ActorId, string ActorName, IReadOnlyList<string> ActorRoles, string? Remarks)
    : IRequest<Result>;

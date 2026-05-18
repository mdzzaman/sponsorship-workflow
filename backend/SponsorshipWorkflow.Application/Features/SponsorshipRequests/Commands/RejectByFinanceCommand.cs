using MediatR;
using SponsorshipWorkflow.Application.Common;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record RejectByFinanceCommand(Guid RequestId, string FinanceId, string FinanceName, string Remarks)
    : IRequest<Result>;

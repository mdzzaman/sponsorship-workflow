using MediatR;
using SponsorshipWorkflow.Domain.Common;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

public record ApproveByFinanceCommand(Guid RequestId, string FinanceId, string FinanceName, string? Remarks)
    : IRequest<Result>;

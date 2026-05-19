using FluentValidation;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandValidators;

public class ApproveByFinanceCommandValidator : AbstractValidator<ApproveByFinanceCommand>
{
    public ApproveByFinanceCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request ID is required.");
        RuleFor(x => x.Remarks).MaximumLength(2000).When(x => x.Remarks != null);
    }
}

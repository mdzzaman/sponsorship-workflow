using FluentValidation;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandValidators;

public class RejectByManagerCommandValidator : AbstractValidator<RejectByManagerCommand>
{
    public RejectByManagerCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty().WithMessage("Request ID is required.");

        RuleFor(x => x.Remarks)
            .NotEmpty().WithMessage("Remarks are required when rejecting.")
            .MinimumLength(10).WithMessage("Rejection remarks must be at least 10 characters.");
    }
}

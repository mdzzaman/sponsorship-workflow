using FluentValidation;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandValidators;

public class CancelRequestCommandValidator : AbstractValidator<CancelRequestCommand>
{
    public CancelRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request ID is required.");
    }
}

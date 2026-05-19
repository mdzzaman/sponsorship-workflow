using FluentValidation;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandValidators;

public class SubmitRequestCommandValidator : AbstractValidator<SubmitRequestCommand>
{
    public SubmitRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request ID is required.");
    }
}

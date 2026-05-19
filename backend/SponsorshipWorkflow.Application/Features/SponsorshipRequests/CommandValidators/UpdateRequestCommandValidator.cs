using FluentValidation;
using SponsorshipWorkflow.Application.Features.SponsorshipRequests.Commands;

namespace SponsorshipWorkflow.Application.Features.SponsorshipRequests.CommandValidators;

public class UpdateRequestCommandValidator : AbstractValidator<UpdateRequestCommand>
{
    public UpdateRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty().WithMessage("Request ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.");

        RuleFor(x => x.SponsorshipTypeId)
            .NotEmpty().WithMessage("Sponsorship type is required.");

        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("Event name is required.")
            .MaximumLength(200).WithMessage("Event name must not exceed 200 characters.");

        RuleFor(x => x.EventDate)
            .GreaterThan(DateTime.UtcNow.Date).WithMessage("Event date must be in the future.");

        RuleFor(x => x.RequestedAmount)
            .GreaterThan(0).WithMessage("Requested amount must be greater than zero.");

        RuleFor(x => x.Justification)
            .NotEmpty().WithMessage("Justification is required.")
            .MinimumLength(20).WithMessage("Justification must be at least 20 characters.")
            .MaximumLength(2000).WithMessage("Justification must not exceed 2000 characters.");

        RuleFor(x => x.ExpectedBenefit)
            .MaximumLength(2000).WithMessage("Expected benefit must not exceed 2000 characters.")
            .When(x => x.ExpectedBenefit is not null);

        RuleFor(x => x.Remarks)
            .MaximumLength(1000).WithMessage("Remarks must not exceed 1000 characters.")
            .When(x => x.Remarks is not null);
    }
}

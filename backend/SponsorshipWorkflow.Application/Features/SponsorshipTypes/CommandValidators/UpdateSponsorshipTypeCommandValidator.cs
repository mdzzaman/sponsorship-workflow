using FluentValidation;
using SponsorshipWorkflow.Application.Features.SponsorshipTypes.Commands;

namespace SponsorshipWorkflow.Application.Features.SponsorshipTypes.CommandValidators;

public class UpdateSponsorshipTypeCommandValidator : AbstractValidator<UpdateSponsorshipTypeCommand>
{
    public UpdateSponsorshipTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sponsorship type ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sponsorship type name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}

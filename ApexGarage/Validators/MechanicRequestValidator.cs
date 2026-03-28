using ApexGarage.DTOs.Mechanics;
using FluentValidation;

namespace ApexGarage.Validators;

public class MechanicRequestValidator : AbstractValidator<MechanicRequest>
{
    public MechanicRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(150).WithMessage("Full name must not exceed 150 characters.");

        RuleFor(x => x.Specialty)
            .NotEmpty().WithMessage("Specialty is required.")
            .MaximumLength(100).WithMessage("Specialty must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[\d\s\-\(\)]{8,20}$").WithMessage("Phone number format is invalid.");
    }
}

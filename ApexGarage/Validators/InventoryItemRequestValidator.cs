using ApexGarage.DTOs.Inventory;
using FluentValidation;

namespace ApexGarage.Validators;

public class InventoryItemRequestValidator : AbstractValidator<InventoryItemRequest>
{
    public InventoryItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Part name is required.")
            .MaximumLength(100).WithMessage("Part name must not exceed 100 characters.");

        RuleFor(x => x.PartNumber)
            .NotEmpty().WithMessage("Part number is required.")
            .MaximumLength(50).WithMessage("Part number must not exceed 50 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(50).WithMessage("Category must not exceed 50 characters.");
    }
}

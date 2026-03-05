using FluentValidation;
using SmartInventory.Services.DTOs;

namespace SmartInventory.Services.Validators
{
    /// <summary>
    /// Validates CreateProductDto before sending to service layer.
    /// </summary>
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Valid CategoryId is required.");
        }
    }
}
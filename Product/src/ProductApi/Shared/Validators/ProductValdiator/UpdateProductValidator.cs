using FluentValidation;
using ProductApi.Shared.ProductDtos;

namespace ProductApi.Shared.Validators.ProductValdiator;
public class UpdateProductValidator : AbstractValidator<UpdateProductDto> {
    public UpdateProductValidator() {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(20);
        RuleFor(x => x.SerialNumber)
            .NotEmpty()
            .MaximumLength(20);
        RuleFor(x => x.Price)
            .InclusiveBetween(0, decimal.MaxValue);
        RuleFor(x => x.Stock)
            .InclusiveBetween(0, int.MaxValue);
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(20);
        RuleFor(x => x.Colors)
            .NotEmpty()
            .MaximumLength(10);
        RuleFor(x => x.Weight)
            .NotEmpty()
            .InclusiveBetween(0, int.MaxValue);
        RuleFor(x => x.Measurements)
            .NotEmpty()
            .MaximumLength(20);
        RuleFor(x => x.DispatchTime)
            .NotEmpty();
        RuleFor(x => x.Brand)
            .NotEmpty()
            .MaximumLength(30);
    }
}
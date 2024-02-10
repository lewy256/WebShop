using FluentValidation;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Service.Validators.ProductValdiator.V1;
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
        RuleFor(x => x.Color)
            .MaximumLength(10);
        RuleFor(x => x.Weight)
            .InclusiveBetween(0, int.MaxValue);
        RuleFor(x => x.Size)
            .MaximumLength(10);
    }
}
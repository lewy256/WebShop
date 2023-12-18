using FluentValidation;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Model.Validators;
public class UpdateProductValidator : AbstractValidator<UpdateProductDto> {
    public UpdateProductValidator() {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(3);

    }
}

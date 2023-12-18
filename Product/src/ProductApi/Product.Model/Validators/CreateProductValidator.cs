using FluentValidation;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Model.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto> {
    public CreateProductValidator() {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(3);

    }
}

using FluentValidation;
using ProductApi.Shared.Model.ProductDtos;

namespace ProductApi.Model.Validators;
public class ProductParametersValidator : AbstractValidator<ProductParameters> {
    public ProductParametersValidator() {
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxPrice)
            .GreaterThan(x => x.MinPrice);
    }
}


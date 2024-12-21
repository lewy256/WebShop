using FluentValidation;
using ProductApi.Extensions;
using ProductApi.Shared.ProductDtos;

namespace ProductApi.Shared.Validators.ProductValdiator;
public class ProductParametersValidator : AbstractValidator<ProductParameters> {
    public ProductParametersValidator() {
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxPrice)
            .GreaterThan(x => x.MinPrice)
            .LessThanOrEqualTo(decimal.MaxValue);
        RuleFor(x => x.SearchTerm)
            .MaximumLength(20);
        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(50);
        RuleFor(x => x.OrderBy)
            .MaximumLength(30)
            .InOrNull("price", "productName", "price desc", "productName desc");
    }
}
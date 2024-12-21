using FluentValidation;
using ProductApi.Extensions;
using ProductApi.Shared.PriceHistoryDtos;

namespace ProductApi.Shared.Validators.PriceHistoryValidator;
public class PriceHistoryParametersValidator : AbstractValidator<PriceHistoryParameters> {
    public PriceHistoryParametersValidator() {
        RuleFor(x => x.MaxPrice)
            .LessThanOrEqualTo(decimal.MaxValue)
            .GreaterThan(x => x.MinPrice);
        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate);
        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(50);
        RuleFor(x => x.OrderBy)
           .MaximumLength(30)
           .InOrNull("priceValue", "startDate", "endDate", "priceValue desc", "startDate desc", "endDate desc");

    }
}
using FluentValidation;
using ProductApi.Service.Extensions;
using ProductApi.Shared.Model.PriceHistoryDtos;

namespace ProductApi.Service.Validators.PriceHistoryValidator;
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
            .InclusiveBetween(10, 50);
        RuleFor(x => x.OrderBy)
           .MaximumLength(20)
           .InOrNull("priceValue", "startDate", "endDate", "priceValue desc", "startDate desc", "endDate desc");

    }
}
using FluentValidation;
using ProductApi.Shared.PriceHistoryDtos;

namespace ProductApi.Shared.Validators.PriceHistoryValidator;
public class CreatePriceHistoryValidator : AbstractValidator<CreatePriceHistoryDto> {
    public CreatePriceHistoryValidator() {
        RuleFor(x => x.PriceValue)
            .InclusiveBetween(0, decimal.MaxValue);
        RuleFor(x => x.StartDate)
            .NotEmpty();
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate);

    }
}
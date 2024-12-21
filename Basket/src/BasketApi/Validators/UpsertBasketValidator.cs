using BasketApi.Shared;
using FluentValidation;

namespace BasketApi.Validators;

public class UpsertBasketValidator : AbstractValidator<UpsertBasketDto> {
    public UpsertBasketValidator() {
        RuleForEach(x => x.Items)
            .SetValidator(new BasketItemValidator());
    }
}


using BasketApi.Shared;
using FluentValidation;

namespace BasketApi.Validators;

public class UpdateBasketValidator : AbstractValidator<UpdateBasketDto> {
    public UpdateBasketValidator() {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new BasketItemValidator());
    }
}


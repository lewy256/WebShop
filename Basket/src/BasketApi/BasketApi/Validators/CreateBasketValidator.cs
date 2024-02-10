using BasketApi.Shared;
using FluentValidation;

namespace BasketApi.Validators;
public class CreateBasketValidator : AbstractValidator<CreateBasketDto> {
    public CreateBasketValidator() {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}


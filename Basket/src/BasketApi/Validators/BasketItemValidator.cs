using BasketApi.Entities;
using FluentValidation;

namespace BasketApi.Validators;

public class BasketItemValidator : AbstractValidator<BasketItem> {
    public BasketItemValidator() {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Price)
           .NotEmpty();
        RuleFor(x => x.Name)
           .NotEmpty();
        RuleFor(x => x.Quantity)
           .NotEmpty();
        RuleFor(x => x.ImageUrl)
           .NotEmpty();
    }
}
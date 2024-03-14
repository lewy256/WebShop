using BasketApi.Models;
using FluentValidation;

namespace BasketApi.Validators;

public class BasketItemValidator : AbstractValidator<BasketItem> {
    public BasketItemValidator() {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.ProductId)
            .NotEmpty();
        RuleFor(x => x.Price)
           .NotEmpty();
        RuleFor(x => x.ProductName)
           .NotEmpty();
        RuleFor(x => x.Quantity)
           .NotEmpty();
        RuleFor(x => x.ImageUrl)
         .NotEmpty();
    }
}
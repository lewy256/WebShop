using FluentValidation;
using ProductApi.Model.Entities;

namespace ProductApi.Model.Validators;
public class CreateCategoryValidator : AbstractValidator<Category> {
    public CreateCategoryValidator() {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .MaximumLength(3);

    }
}


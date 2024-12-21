using FluentValidation;
using ProductApi.Shared.CategoryDtos;

namespace ProductApi.Shared.Validators.CategoryValidator;
public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto> {
    public CreateCategoryValidator() {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .MaximumLength(20);

    }
}


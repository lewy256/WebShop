using FluentValidation;
using ProductApi.Shared.CategoryDtos;

namespace ProductApi.Shared.Validators.CategoryValidator;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto> {
    public UpdateCategoryValidator() {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .MaximumLength(20);

    }
}
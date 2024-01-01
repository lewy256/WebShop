using FluentValidation;
using ProductApi.Shared.Model.CategoryDtos;

namespace ProductApi.Service.Validators.CategoryValidator;
public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto> {
    public CreateCategoryValidator() {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .MaximumLength(20);

    }
}


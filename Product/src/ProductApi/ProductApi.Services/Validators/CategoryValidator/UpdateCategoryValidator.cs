using FluentValidation;
using ProductApi.Shared.Model.CategoryDtos;

namespace ProductApi.Service.Validators.CategoryValidator;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto> {
    public UpdateCategoryValidator() {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .MaximumLength(20);

    }
}
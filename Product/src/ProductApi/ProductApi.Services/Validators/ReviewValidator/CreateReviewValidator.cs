using FluentValidation;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service.Validators.ReviewValidator;
public class CreateReviewValidator : AbstractValidator<CreateReviewDto> {
    public CreateReviewValidator() {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 5);
    }
}
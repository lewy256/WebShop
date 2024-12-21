using FluentValidation;
using ProductApi.Shared.ReviewDtos;

namespace ProductApi.Shared.Validators.ReviewValidator;
public class UpdateReviewValidator : AbstractValidator<UpdateReviewDto> {
    public UpdateReviewValidator() {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 5);
    }
}
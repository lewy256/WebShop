using FluentValidation;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service.Validators.ReviewValidator;
public class UpdateReviewValidator : AbstractValidator<UpdateReviewDto> {
    public UpdateReviewValidator() {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 5);
    }
}
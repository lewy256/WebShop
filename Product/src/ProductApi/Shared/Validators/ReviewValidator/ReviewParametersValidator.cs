using FluentValidation;
using ProductApi.Extensions;
using ProductApi.Shared.ReviewDtos;

namespace ProductApi.Shared.Validators.ReviewValidator;
public class ReviewParametersValidator : AbstractValidator<ReviewParameters> {
    public ReviewParametersValidator() {
        RuleFor(x => x.Rating)
           .InclusiveBetween(0, 5);
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate);
        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(50);
        RuleFor(x => x.OrderBy)
           .MaximumLength(30)
           .InOrNull("rating", "reviewDate", "rating desc", "reviewDate desc");
    }
}

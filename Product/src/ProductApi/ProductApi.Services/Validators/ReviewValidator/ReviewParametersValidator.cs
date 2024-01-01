using FluentValidation;
using ProductApi.Service.Extensions;
using ProductApi.Shared.Model.ReviewDtos;

namespace ProductApi.Service.Validators.ReviewValidator;
public class ReviewParametersValidator : AbstractValidator<ReviewParameters> {
    public ReviewParametersValidator() {
        RuleFor(x => x.Rating)
           .InclusiveBetween(0, 5)
           .LessThanOrEqualTo(5);
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate);
        RuleFor(x => x.PageSize)
            .InclusiveBetween(10, 50);
        RuleFor(x => x.OrderBy)
           .MaximumLength(20)
           .InOrNull("rating", "reviewDate", "rating desc", "reviewDate desc");
    }
}

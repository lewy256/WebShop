using FluentValidation;
using FluentValidation.Results;
using OrderApi.Commands;

namespace OrderApi.Validators;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand> {
    public CreateOrderCommandValidator() {
        RuleFor(c => c.Order.TotalPrice).NotEmpty();

        RuleFor(c => c.Order.AddressId).NotEmpty();
    }

    public override ValidationResult Validate(ValidationContext<CreateOrderCommand> context) {
        return context.InstanceToValidate.Order is null
            ? new ValidationResult(new[] { new ValidationFailure("CreateOrderDto",
                "CreateOrderDto object is null") })
            : base.Validate(context);
    }
}

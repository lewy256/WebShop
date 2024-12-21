using FluentValidation;
using IdentityApi.Shared;

namespace IdentityApi.Validators;
public class RegistrationUserDtoValidator : AbstractValidator<RegistrationUserDto> {
    public RegistrationUserDtoValidator() {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+\d{2}\d{3}\d{3}\d{3}$")
            .WithMessage("Phone number must be in the format +XXXXXXXXXXX (e.g., +22423492123).");
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);
    }
}
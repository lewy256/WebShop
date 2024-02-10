using FluentValidation;
using IdentityApi.Shared;

namespace IdentityApi.Validators;
public class RegistrationUserDtoValidator : AbstractValidator<RegistrationUserDto> {
    public RegistrationUserDtoValidator() {
        RuleFor(x => x.UserName)
            .NotEmpty();
        RuleFor(x => x.Password)
            .NotEmpty();
    }
}


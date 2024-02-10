using FluentValidation;
using IdentityApi.Shared;

namespace IdentityApi.Validators;
public class AuthUserDtoValidator : AbstractValidator<AuthenticationUserDto> {
    public AuthUserDtoValidator() {
        RuleFor(x => x.UserName)
            .NotEmpty();
        RuleFor(x => x.Password)
            .NotEmpty();
    }
}


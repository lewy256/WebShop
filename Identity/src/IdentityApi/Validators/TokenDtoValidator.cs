using FluentValidation;
using IdentityApi.Shared;

namespace IdentityApi.Validators;
public class TokenDtoValidator : AbstractValidator<TokenDto> {
    public TokenDtoValidator() {
        RuleFor(x => x.AccessToken)
            .NotEmpty();
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}


using Bogus;
using Bogus.Extensions;
using IdentityApi.Shared;

namespace IdentityApi.IntegrationTests;

public class RegistrationUserFaker : Faker<RegistrationUserDto> {
    public RegistrationUserFaker() {
        RuleFor(x => x.FirstName, f => f.Person.FirstName.ClampLength(30, 50));
        RuleFor(x => x.LastName, f => f.Person.LastName.ClampLength(30, 50));
        RuleFor(x => x.UserName, f => f.Person.UserName.ClampLength(30, 50));
        RuleFor(x => x.Password, f => f.Internet.Password(15));
        RuleFor(x => x.Email, f => f.Person.Email);
        RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber("+###########"));
    }
}

public class AuthenticationUserFaker : Faker<AuthenticationUserDto> {
    public AuthenticationUserFaker() {
        RuleFor(x => x.UserName, f => f.Person.UserName);
        RuleFor(x => x.Password, f => f.Internet.Password());
    }
}
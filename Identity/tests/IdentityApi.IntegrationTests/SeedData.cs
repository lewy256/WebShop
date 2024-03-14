using Bogus;
using IdentityApi.Shared;

namespace IdentityApi.IntegrationTests;

public class RegistrationUserFaker : Faker<RegistrationUserDto> {
    public RegistrationUserFaker() {
        RuleFor(x => x.FirstName, f => f.Person.FirstName);
        RuleFor(x => x.LastName, f => f.Person.LastName);
        RuleFor(x => x.UserName, f => f.Person.UserName);
        RuleFor(x => x.Password, f => f.Internet.Password(15));
        RuleFor(x => x.Email, f => f.Person.Email);
        RuleFor(x => x.PhoneNumber, f => f.Person.Phone);
        RuleFor(x => x.Roles, f => ["Administrator"]);
    }
}

public class AuthenticationUserFaker : Faker<AuthenticationUserDto> {
    public AuthenticationUserFaker() {
        RuleFor(x => x.UserName, f => f.Person.UserName);
        RuleFor(x => x.Password, f => f.Internet.Password());
    }
}
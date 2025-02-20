using Bogus;
using CareerCompass.Api.Contracts.Users;

namespace CareerCompass.Tests.Fakers.Api;

public class LoginRequestFaker : Faker<LoginRequest>
{
    public LoginRequestFaker()
    {
        RuleFor(x => x.Email, f => f.Person.Email);
        RuleFor(x => x.Password, f => f.Internet.Password());
        CustomInstantiator(f => new LoginRequest("", ""));
    }
}
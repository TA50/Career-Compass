using Bogus;
using CareerCompass.Core.Users.Commands.Register;

namespace CareerCompass.Tests.Fakers.Core;

public sealed class RegisterCommandFaker : Faker<RegisterCommand>
{
    public RegisterCommandFaker()
    {
        CustomInstantiator(faker => new RegisterCommand("", "", "", "", ""));
        RuleFor(x => x.FirstName, faker => faker.Person.FirstName);
        RuleFor(x => x.LastName, faker => faker.Person.LastName);
        RuleFor(x => x.Email, faker => faker.Person.Email);
        RuleFor(x => x.Password, faker => faker.Internet.Password(regexPattern: "[A-Za-z\\d@$!%*?&\\-_]"));
        RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);
    }
}
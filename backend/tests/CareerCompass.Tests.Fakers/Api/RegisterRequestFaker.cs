using Bogus;
using CareerCompass.Api.Contracts.Users;

namespace CareerCompass.Tests.Fakers.Api;

public class RegisterRequestFaker : Faker<RegisterRequest>
{
    public RegisterRequestFaker()
    {
        RuleFor(r => r.FirstName, f => f.Person.FirstName);
        RuleFor(r => r.LastName, f => f.Person.LastName);
        RuleFor(r => r.Email, f => f.Person.Email);
        RuleFor(r => r.Password, f => f.StrongPassword());
        RuleFor(r => r.ConfirmPassword, (f, r) => r.Password);
    }
}
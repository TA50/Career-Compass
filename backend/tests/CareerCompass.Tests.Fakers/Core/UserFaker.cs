using Bogus;
using CareerCompass.Core.Users;

namespace CareerCompass.Tests.Fakers.Core;

public class UserFaker : Faker<User>
{
    public override User Generate(string ruleSets = null!)
    {
        var faker = new Faker();
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var passwordHash = faker.Random.Chars(count: 64).ToString() ?? password;
        var firstName = faker.Person.FirstName;
        var lastName = faker.Person.LastName;

        return User.Create(
            email,
            passwordHash,
            firstName,
            lastName
        );
    }

    public static string GenerateDifferentPassword(string password)
    {
        string differentPassword;
        do
        {
            differentPassword = new Faker().Internet.Password();
        } while (differentPassword == password);

        return differentPassword;
    }
}
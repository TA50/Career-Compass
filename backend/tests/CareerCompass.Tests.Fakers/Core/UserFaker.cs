using Bogus;
using CareerCompass.Core.Users;

namespace CareerCompass.Tests.Fakers.Core;

public class UserFaker : Faker<User>
{
    public override User Generate(string ruleSets = null!)
    {
        var faker = new Faker();
        var email = faker.Internet.Email();
        var password = faker.StrongPassword();
        var firstName = faker.Person.FirstName;
        var lastName = faker.Person.LastName;

        return User.Create(
            email,
            password,
            firstName,
            lastName
        );
    }


    public static string GenerateDifferentCode(string code)
    {
        string differentCode;
        do
        {
            differentCode = new Faker().Random.AlphaNumeric(6);
        } while (differentCode == code);

        return differentCode;
    }

    public static string GenerateDifferentPassword(string password)
    {
        string differentPassword;
        do
        {
            differentPassword = new Faker().StrongPassword();
        } while (differentPassword == password);

        return differentPassword;
    }

    public static string GenerateDifferentEmail(string email)
    {
        string differentEmail;
        do
        {
            differentEmail = new Faker().Internet.Email();
        } while (differentEmail == email);

        return differentEmail;
    }
}
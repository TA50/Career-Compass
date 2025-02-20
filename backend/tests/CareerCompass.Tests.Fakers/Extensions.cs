using Bogus;
using CareerCompass.Core.Common;

namespace CareerCompass.Tests.Fakers;


public static class Extensions
{
    public static string StrongPassword(this Faker faker)
    {
        var lowerLetter = faker.Internet.Password(regexPattern: $"[a-z]", length: 1);
        var upperLetter = faker.Internet.Password(regexPattern: $"[A-Z]", length: 1);
        var number = faker.Internet.Password(regexPattern: $"[0-9]", length: 1);
        var symbol = faker.Internet.Password(regexPattern: $"[{RegexPatterns.PasswordSpecialCharacters}]", length: 1);
        var totalLength = faker.Random.Int(Limits.MinPasswordLength + 5, Limits.MaxPasswordLength);

        int remainingLength = totalLength - 5;
        var padding = faker.Internet.Password(remainingLength, false,
            $"[A-Za-z0-9{RegexPatterns.PasswordSpecialCharacters}]");

        var fullPassword = symbol + lowerLetter + upperLetter + number + symbol + padding;


        return new string(fullPassword.ToCharArray().OrderBy(_ => faker.Random.Int()).ToArray());
    }
}
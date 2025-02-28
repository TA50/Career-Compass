using Bogus;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;

namespace CareerCompass.Tooling.Seeders;

class UserSeeder(AppDbContext dbContext, ICryptoService cryptoService, ILoggerAdapter<UserSeeder> logger)
    : IDatabaseSeeder
{
    const int EmailConfirmationCodeLifetime = 24;


    public async Task SeedAsync(int count = 1)
    {
        logger.LogInformation("Seeding {Count} users...", count);
        var users = Enumerable.Range(0, count).Select(_ => Generate()).ToList();

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} users.", count);
    }

    private User Generate(bool confirmedEmail = true)
    {
        var faker = new Faker();
        var email = faker.Internet.Email();
        var password = "Password123!";
        var hashedPassword = cryptoService.Hash(password);
        var firstName = faker.Person.FirstName;
        var lastName = faker.Person.LastName;

        var user = User.Create(
            email,
            hashedPassword,
            firstName,
            lastName
        );
        if (confirmedEmail)
        {
            var coe = user.GenerateEmailConfirmationCode(TimeSpan.FromHours(EmailConfirmationCodeLifetime));
            user.ConfirmEmail(coe);
        }

        return user;
    }

    private string GenerateStrongPassword()
    {
        var faker = new Faker();
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
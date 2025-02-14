using Bogus;

var faker = new Faker<RegisterCommand>()
    .CustomInstantiator((faker) => new RegisterCommand("", "", "", "", ""))
    .RuleFor(x => x.FirstName, faker => faker.Person.FirstName)
    .RuleFor(x => x.LastName, faker => faker.Person.LastName)
    .RuleFor(x => x.Email, faker => faker.Person.Email)
    .RuleFor(x => x.Password,
        faker => faker.Internet.Password(regexPattern:"[A-Za-z\\d@$!%*?&\\-_]")
    )
    .RuleFor(x => x.ConfirmPassword, (f, u) => u.Password);


var r = faker.Generate();
Console.WriteLine(r);

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
);
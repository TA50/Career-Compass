namespace CareerCompass.Api.Contracts.Users;

public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email
);
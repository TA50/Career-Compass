namespace CareerCompass.Api.Users.Contracts;

public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    ICollection<string> TagIds,
    ICollection<string> FieldIds
);
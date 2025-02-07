using CareerCompass.Application.Users.Commands.UpdateDetails;

namespace CareerCompass.Api.Users.Contracts;

public record UpdateUserRequest(
    string FirstName,
    string LastName
)
{
    public UpdateUserDetailsCommand ToUpdateUserCommand(string userId)
    {
        return new UpdateUserDetailsCommand(userId, FirstName, LastName);
    }
}
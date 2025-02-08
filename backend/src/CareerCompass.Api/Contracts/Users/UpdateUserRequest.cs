using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.UpdateDetails;

namespace CareerCompass.Api.Contracts.Users;

public record UpdateUserRequest(
    string FirstName,
    string LastName
)
{
    public UpdateUserDetailsCommand ToUpdateUserCommand(UserId userId)
    {
        return new UpdateUserDetailsCommand(userId, FirstName, LastName);
    }
}
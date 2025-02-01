using ErrorOr;

namespace CareerCompass.Application.Users;

public static class UserErrors
{
    public static Error UserNotFound(UserId userId)
    {
        return Error.NotFound("User not found", $"User with id ({userId}) was not found ",
            new Dictionary<string, object>()
            {
                { "userId", userId }
            }
        );
    }
}
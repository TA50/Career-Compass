using CareerCompass.Application.Common;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Tags;

public static class TagErrors
{
    public static Error TagValidation_UserNotFound(UserId userId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() }
        };
        return Error.Validation("The provided user for this tag  does not exist.",
            $"User with id {userId} not found.", metadata);
    }


    public static Error TagValidation_TagNameAlreadyExists(UserId userId, string tagName)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            { "TagName", tagName }
        };
        return Error.Conflict(
            "Tag with the same name already exists",
            $"Tag with name {tagName} already exists for user with id {userId}",
            metadata);
    }
}
using CareerCompass.Application.Common;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Tags;

public static class TagErrors
{
    public static Error TagValidation_UserNotFound(UserId userId) =>
        Error.Validation("TagValidationUserNotFound", $"User with id {userId} not found.");

    public static Error TagValidation_TagNameAlreadyExists(UserId userId, string tagName) => Error.Conflict(
        "TagNameAlreadyExists",
        $"Tag with name {tagName} already exists for user with id {userId}");
}
using CareerCompass.Application.Common;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Tags;

public static class TagErrorCode
{
    private static readonly string _tagErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.Tag);

    public static class Creation
    {
        private static readonly string _prefix = $"{_tagErrorPrefix}.{10}";

        public static string UserNotFound => $"{_prefix}.10";
        public static string TagNameAlreadyExists => $"{_prefix}.20";
    }
}

public static class TagErrors
{
    public static Error TagValidation_UserNotFound(UserId userId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            { ErrorMetaDataKey.Title, "Tag Creation:The provided user for this tag  does not exist. " }
        };
        return Error.Validation(TagErrorCode.Creation.UserNotFound,
            $"User with id {userId} not found.", metadata);
    }


    public static Error TagValidation_TagNameAlreadyExists(UserId userId, string tagName)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            { "TagName", tagName },
            { ErrorMetaDataKey.Title, "Tag Creation:Tag with the same name already exists for the user." }
        };
        return Error.Conflict(
            TagErrorCode.Creation.TagNameAlreadyExists,
            $"Tag with name {tagName} already exists for user with id {userId}",
            metadata);
    }
}
using CareerCompass.Core.Common;
using CareerCompass.Core.Users;
using ErrorOr;

namespace CareerCompass.Core.Tags;

public static class TagErrorCode
{
    private static readonly string _tagErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.Tag);

    public static class Creation
    {
        private static readonly string _prefix = $"{_tagErrorPrefix}.{10}";

        public static string UserNotFound => $"{_prefix}.{10}";
        public static string TagNameAlreadyExists => $"{_prefix}.{20}";

        public static string FailedToCreateTag => $"{_prefix}.{30}";
    }

    public static class Read
    {
        private static readonly string _prefix = $"{_tagErrorPrefix}.{20}";
        public static string TagNotFound => $"{_prefix}.10";
    }

    public static class Delete
    {
        private static readonly string _prefix = $"{_tagErrorPrefix}.{30}";
        public static string TagNotFound => $"{_prefix}.10";
        public static string OperationFailed => $"{_prefix}.20";
    }
}

public static class TagErrors
{
    #region Creation

    public static Error TagCreation_UserNotFound(UserId userId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            { ErrorMetaDataKey.Title, "Tag Creation:The provided user for this tag  does not exist. " }
        };
        return Error.Validation(TagErrorCode.Creation.UserNotFound,
            $"User with id {userId} not found.", metadata);
    }


    public static Error TagCreation_TagNameAlreadyExists(UserId userId, string tagName)
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

    public static Error TagCreation_FailedToCreateTag(UserId userId, string tagName)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            { "TagName", tagName },
            { ErrorMetaDataKey.Title, "Tag Creation:Failed to create tag." }
        };
        return Error.Validation(TagErrorCode.Creation.FailedToCreateTag,
            $"Failed to create tag with name {tagName} for user with id {userId}", metadata);
    }

    #endregion

    #region Read

    public static Error TagRead_TagNotFound(UserId userId, TagId tagId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            { "TagId", tagId.ToString() },
            { ErrorMetaDataKey.Title, "Tag Read: Tag was not found " }
        };
        return Error.NotFound(TagErrorCode.Read.TagNotFound,
            $"Tag with  userId {userId} and tagId {tagId} was not found.", metadata);
    }

    #endregion

    #region Delete

    public static Error TagDelete_TagNotFound(TagId tagId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "TagId", tagId.ToString() },
            { ErrorMetaDataKey.Title, "Tag Delete: Tag was not found " }
        };
        return Error.Validation(TagErrorCode.Delete.TagNotFound,
            $"Tag with tagId {tagId} was not found.", metadata);
    }


    public static Error TagDelete_OperationFailed(TagId tagId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "TagId", tagId.ToString() },
            { ErrorMetaDataKey.Title, "Tag Delete: Failed to delete tag " }
        };
        return Error.Validation(TagErrorCode.Delete.OperationFailed,
            $"Failed to delete tag with tagId {tagId}.", metadata);
    }

    #endregion
}
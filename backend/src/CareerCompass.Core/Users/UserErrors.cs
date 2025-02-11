using CareerCompass.Core.Common;
using ErrorOr;

namespace CareerCompass.Core.Users;

public static class UserErrorCode
{
    private static readonly string _userErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.User);

    public static class Query
    {
        private static readonly string _prefix = $"{_userErrorPrefix}.{20}";

        public static string UserNotFound => $"{_prefix}.10";
    }

    public static class Modification
    {
        private static readonly string _prefix = $"{_userErrorPrefix}.{30}";

        public static string UserNotFound => $"{_prefix}.10";
        public static string ModificationFailed => $"{_prefix}.20";
    }
}

public static class UserErrors
{
    public static Error UserQuery_UserNotFound(UserId userId)
    {
        return Error.NotFound(UserErrorCode.Query.UserNotFound,
            $"User with id ({userId}) was not found ",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "User not found" }
            }
        );
    }

    #region Modification

    public static Error UserModification_UserNotFound(UserId userId)
    {
        return Error.NotFound(UserErrorCode.Modification.UserNotFound,
            $"User with id ({userId}) was not found ",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "User not found" }
            }
        );
    }

    public static Error UserModification_ModificationFailed(UserId userId)
    {
        return Error.NotFound(UserErrorCode.Modification.ModificationFailed,
            $"User modification failed for user with id ({userId})",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "User modification failed" }
            }
        );
    }

    #endregion
}
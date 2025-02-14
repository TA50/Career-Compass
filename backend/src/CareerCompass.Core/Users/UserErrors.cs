using CareerCompass.Core.Common;
using ErrorOr;

namespace CareerCompass.Core.Users;

public static class UserErrorCode
{
    private static readonly string _userErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.User);

    public static class Creation
    {
        private static readonly string _prefix = $"{_userErrorPrefix}.{10}";

        public static string UserWithEmailExists => $"{_prefix}.10";

        public static string CreationFailed => $"{_prefix}.20";
    }

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

    public static class Delete
    {
        private static readonly string _prefix = $"{_userErrorPrefix}.{40}";

        public static string UserNotFound => $"{_prefix}.10";
    }

    public static class EmailConfirmation
    {
        private static readonly string _prefix = $"{_userErrorPrefix}.{50}";
        public static string UserNotFound => $"{_prefix}.10";
        public static string InvalidEmailConfirmationCode => $"{_prefix}.20";

        public static string ConfirmationFailed => $"{_prefix}.30";
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
        return Error.Failure(UserErrorCode.Modification.ModificationFailed,
            $"User modification failed for user with id ({userId})",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "User modification failed" }
            }
        );
    }

    #endregion

    #region Creation

    public static Error UserCreation_CreationFailed(string email)
    {
        return Error.Failure(UserErrorCode.Creation.CreationFailed,
            $"User creation failed for user with email ({email})",
            new Dictionary<string, object>()
            {
                { "email", email },
                { ErrorMetaDataKey.Title, "User creation failed" }
            }
        );
    }

    public static Error UserCreation_UserWithEmailExists(string email)
    {
        return Error.Conflict(UserErrorCode.Creation.UserWithEmailExists,
            $"User with email ({email}) already exists",
            new Dictionary<string, object>()
            {
                { "email", email },
                { ErrorMetaDataKey.Title, "User with email already exists" }
            }
        );
    }

    #endregion

    #region EmailConfirmation

    public static Error UserEmailConfirmation_InvalidEmailConfirmationCode(UserId userId)
    {
        return Error.Validation(UserErrorCode.EmailConfirmation.InvalidEmailConfirmationCode,
            $"Email confirmation failed for user with userId ({userId}). the provided confirmation code is not correct",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "Email confirmation failed, invalid confirmation code" }
            }
        );
    }


    public static Error UserEmailConfirmation_UserNotFound(UserId userId)
    {
        return Error.Validation(UserErrorCode.EmailConfirmation.UserNotFound,
            $"Email confirmation failed. User with userId ({userId}) was not found",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "Email confirmation failed, user not found" }
            }
        );
    }

    public static Error UserEmailConfirmation_ConfirmationFailed(UserId userId)
    {
        return Error.Validation(UserErrorCode.EmailConfirmation.ConfirmationFailed,
            $"Email confirmation failed for user with userId ({userId})",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "Email confirmation failed" }
            }
        );
    }

    #endregion
}
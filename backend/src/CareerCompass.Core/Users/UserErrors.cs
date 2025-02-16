using CareerCompass.Core.Common;
using ErrorOr;

namespace CareerCompass.Core.Users;

public static class UserErrorCode
{
    private static class Prefixes
    {
        public static readonly string CreationCode = $"{_userErrorPrefix}.{10}";
        public static readonly string QueryCode = $"{_userErrorPrefix}.{20}";
        public static readonly string ModificationCode = $"{_userErrorPrefix}.{30}";
        public static readonly string DeleteCode = $"{_userErrorPrefix}.{40}";
        public static readonly string EmailConfirmationCode = $"{_userErrorPrefix}.{50}";
        public static readonly string LoginCode = $"{_userErrorPrefix}.{60}";
        public static readonly string ForgotPassword = $"{_userErrorPrefix}.{70}";
        public static readonly string ResetPasswordCode = $"{_userErrorPrefix}.{80}";
        public static readonly string ChangeEmailCode = $"{_userErrorPrefix}.{90}";
    }

    private static readonly string _userErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.User);

    public static class Creation
    {
        public static string UserWithEmailExists => $"{Prefixes.CreationCode}.{10}";

        public static string CreationFailed => $"{Prefixes.CreationCode}.{20}";
    }

    public static class Query
    {
        public static string UserNotFound => $"{Prefixes.QueryCode}.{10}";
    }

    public static class Modification
    {
        public static string UserNotFound => $"{Prefixes.ModificationCode}.10";
        public static string ModificationFailed => $"{Prefixes.ModificationCode}.20";
    }

    public static class Delete
    {
        public static string UserNotFound => $"{Prefixes.DeleteCode}.10";
    }

    public static class EmailConfirmation
    {
        public static string UserNotFound => $"{Prefixes.EmailConfirmationCode}.10";
        public static string InvalidEmailConfirmationCode => $"{Prefixes.EmailConfirmationCode}.20";

        public static string ConfirmationFailed => $"{Prefixes.EmailConfirmationCode}.30";
    }


    public static class Login
    {
        public static string InvalidCredentials => $"{Prefixes.LoginCode}.10";
    }

    public static class ResetPassword
    {
        public static string InvalidCredentials => $"{Prefixes.ResetPasswordCode}.10";
        public static string OperationFailed => $"{Prefixes.ResetPasswordCode}.20";
    }

    public static class ChangeEmail
    {
        public static string UserNotFound => $"{Prefixes.ChangeEmailCode}.10";
        public static string InvalidCredentials => $"{Prefixes.ChangeEmailCode}.20";
        public static string OperationFailed => $"{Prefixes.ChangeEmailCode}.30";
        public static string EmailAlreadyExists => $"{Prefixes.ChangeEmailCode}.40";
    }
}

public static class UserErrors
{
    #region Query

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

    #endregion

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

    #region Login

    public static Error UserLogin_InvalidCredentials(string email)
    {
        return Error.Conflict(UserErrorCode.Login.InvalidCredentials,
            "Invalid credentials provided. Please check your email and password",
            new Dictionary<string, object>()
            {
                { "email", email },
                { ErrorMetaDataKey.Title, "Invalid credentials" }
            }
        );
    }

    #endregion

    #region Reset Password

    public static Error ResetPassword_InvalidCredentials(UserId userId)
    {
        return Error.Conflict(UserErrorCode.ResetPassword.InvalidCredentials,
            "Invalid credentials provided. Please check your again",
            new Dictionary<string, object>()
            {
                { "userId", userId.ToString() },
                { ErrorMetaDataKey.Title, "Reset password: Invalid credentials" }
            }
        );
    }

    public static Error ResetPassword_OperationFailed(UserId userId)
    {
        return Error.Failure(UserErrorCode.ResetPassword.OperationFailed,
            $"Reset password operation failed for user with id ({userId})",
            new Dictionary<string, object>()
            {
                { "userId", userId.ToString() },
                { ErrorMetaDataKey.Title, "Reset password: Operation failed" }
            }
        );
    }

    #endregion

    #region Change Email

    public static Error ChangeEmail_UserNotFound(UserId userId)
    {
        return Error.NotFound(UserErrorCode.ChangeEmail.UserNotFound,
            $"User with id ({userId}) was not found ",
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { ErrorMetaDataKey.Title, "User not found" }
            }
        );
    }

    public static Error ChangeEmail_InvalidCredentials(UserId userId)
    {
        return Error.Conflict(UserErrorCode.ChangeEmail.InvalidCredentials,
            "Invalid credentials provided. Please check your again",
            new Dictionary<string, object>()
            {
                { "userId", userId.ToString() },
                { ErrorMetaDataKey.Title, "Change email: Invalid credentials" }
            }
        );
    }


    public static Error ChangeEmail_OperationFailed(UserId userId)
    {
        return Error.Failure(UserErrorCode.ChangeEmail.OperationFailed,
            $"Change email operation failed for user with id ({userId})",
            new Dictionary<string, object>()
            {
                { "userId", userId.ToString() },
                { ErrorMetaDataKey.Title, "Change email: Operation failed" }
            }
        );
    }

    public static Error ChangeEmail_EmailAlreadyExists(string email)
    {
        return Error.Conflict(UserErrorCode.ChangeEmail.EmailAlreadyExists,
            $"User with email ({email}) already exists",
            new Dictionary<string, object>()
            {
                { "email", email },
                { ErrorMetaDataKey.Title, "Change email: Email already exists" }
            }
        );
    }

    #endregion
}
using CareerCompass.Application.Common;
using ErrorOr;

namespace CareerCompass.Application.Users;

public static class UserErrorCode
{
    private static readonly string _userErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.User);

    public static class Query
    {
        private static readonly string _prefix = $"{_userErrorPrefix}.{20}";

        public static string UserNotFound => $"{_prefix}.10";
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
}
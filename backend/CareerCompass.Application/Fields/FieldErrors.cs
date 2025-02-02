using CareerCompass.Application.Common;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Fields;

public static class FieldErrorCode
{
    private static string _fieldPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.Field);

    public static class Creation
    {
        private static string _prefix = $"{_fieldPrefix}.{10}";

        public static string NameAlreadyExists = $"{_prefix}.{10}";
        public static string UserNotFound = $"{_prefix}.{20}";
    }
}

public static class FieldErrors
{
    public static Error FieldValidation_NameAlreadyExists(UserId userId, string name)
    {
        return Error.Conflict(FieldErrorCode.Creation.NameAlreadyExists,
            $"Field with name '{name}' already exists for user '{userId}'",
            new Dictionary<string, object>
            {
                { "UserId", userId.ToString() },
                { "Name", name },
                { ErrorMetaDataKey.Title, "FieldCreation: Field with the same name already exists" }
            }
        );
    }


    public static Error FieldValidation_UserNotFound(UserId userId) =>
        Error.Validation(FieldErrorCode.Creation.UserNotFound,
            $"User '{userId}' not found", new Dictionary<string, object>
            {
                { "UserId", userId.ToString() },
                { ErrorMetaDataKey.Title, "FieldCreation:The provided user id for this field does not exist" }
            });
}
using CareerCompass.Core.Common;
using CareerCompass.Core.Users;
using ErrorOr;

namespace CareerCompass.Core.Fields;

public static class FieldErrorCode
{
    private static string _fieldPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.Field);

    public static class Creation
    {
        private static string _prefix = $"{_fieldPrefix}.{10}";

        public static string NameAlreadyExists = $"{_prefix}.{10}";
        public static string UserNotFound = $"{_prefix}.{20}";

        public static string FailedToCreateField = $"{_prefix}.{30}";
    }

    public static class Read
    {
        private static string _prefix = $"{_fieldPrefix}.{20}";
        public static string TagNotFound = $"{_prefix}.{20}";
    }
}

public static class FieldErrors
{
    #region Creation

    public static Error FieldValidation_NameAlreadyExists(UserId userId, string name, string group)
    {
        return Error.Conflict(FieldErrorCode.Creation.NameAlreadyExists,
            $"Field with name '{name}' in the group: '{group}' already exists for user '{userId}'",
            new Dictionary<string, object>
            {
                { "UserId", userId.ToString() },
                { "Name", name },
                { "Group", group },
                { ErrorMetaDataKey.Title, "FieldCreation: Field with the same name already exists" }
            }
        );
    }

    public static Error FieldCreation_FailedToCreateField(UserId userId,
        string name,
        string group
    ) =>
        Error.Validation(FieldErrorCode.Creation.FailedToCreateField,
            $"Failed to create field with name '{name}' and '{group}' for user '{userId}'",
            new Dictionary<string, object>
            {
                { "UserId", userId.ToString() },
                { "Name", name },
                { "Group", group },
                { ErrorMetaDataKey.Title, "FieldCreation: Failed to create field" }
            });


    public static Error FieldValidation_UserNotFound(UserId userId) =>
        Error.Validation(FieldErrorCode.Creation.UserNotFound,
            $"User '{userId}' not found", new Dictionary<string, object>
            {
                { "UserId", userId.ToString() },
                { ErrorMetaDataKey.Title, "FieldCreation:The provided user id for this field does not exist" }
            });

    #endregion

    #region Read

    public static Error FieldRead_FieldNotFound(UserId userId, FieldId fieldId) =>
        Error.Validation(FieldErrorCode.Read.TagNotFound,
            $" with user id : {userId} and field id : {fieldId} does not exist", new Dictionary<string, object>
            {
                { "UserId", userId.ToString() },
                { "FieldId", userId.ToString() },
                { ErrorMetaDataKey.Title, $"FieldRead: Field with the given user id and field id was not found!" }
            });

    #endregion
}
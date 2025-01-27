using System.Runtime.InteropServices.JavaScript;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Fields;

public static class FieldErrors
{
    public static Error FieldValidation_NameAlreadyExists(UserId userId, string name) =>
        Error.Validation("FieldNameAlreadyExists", $"Field with name '{name}' already exists for user '{userId}'");

    public static Error FieldValidation_UserNotFound(UserId userId) =>
        Error.Validation("FieldValidationUserNotFound", $"User '{userId}' not found");
}
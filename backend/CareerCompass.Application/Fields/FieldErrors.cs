using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Fields;

public static class FieldErrors
{
    public static Error FieldValidation_NameAlreadyExists(UserId userId, string name)
    {
        return Error.Conflict("Field with the same name already exists",
            $"Field with name '{name}' already exists for user '{userId}'", new Dictionary<string, object>
            {
                { "UserId", userId.ToString() },
                { "Name", name }
            });
    }


    public static Error FieldValidation_UserNotFound(UserId userId) =>
        Error.Validation("FieldValidation: The provided user id for this field does not exist",
            $"User '{userId}' not found", new Dictionary<string, object>
            {
                { "UserId", userId.ToString() }
            });
}
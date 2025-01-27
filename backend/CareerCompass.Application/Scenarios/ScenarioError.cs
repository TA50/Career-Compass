using CareerCompass.Application.Common;
using CareerCompass.Application.Fields;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Scenarios;

public static class ScenarioError
{
    public static Error ScenarioValidation_TagNotFound(TagId tagId) =>
        Error.Validation("ScenarioValidationTagNotFound", $"Tag with id {tagId} does not exist");

    public static Error ScenarioValidation_FieldNotFound(FieldId fieldId) =>
        Error.Validation("ScenarioValidationFieldNotFound", $"Field with id {fieldId} does not exist");

    public static Error ScenarioValidation_UserNotFound(UserId userId) =>
        Error.Validation("ScenarioValidationUserNotFound", $"User with id {userId} does not exist");
}
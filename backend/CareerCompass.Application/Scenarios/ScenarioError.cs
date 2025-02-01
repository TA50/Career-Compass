using CareerCompass.Application.Fields;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Scenarios;

public static class ScenarioError
{
    public static Error ScenarioValidation_TagNotFound(TagId tagId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "TagId", tagId.ToString() }
        };

        return Error.Validation("ScenarioValidation: The provided tag id for this scenario does not exist",
            $"Tag with id {tagId} does not exist", metadata);
    }


    public static Error ScenarioValidation_FieldNotFound(FieldId fieldId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "FieldId", fieldId.ToString() }
        };
        return Error.Validation("ScenarioValidation: The provided field id for this scenario does not exist",
            $"Field with id {fieldId} does not exist", metadata);
    }


    public static Error ScenarioValidation_UserNotFound(UserId userId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() }
        };
        return Error.Validation("ScenarioValidation: The provided user id for this scenario does not exist",
            $"User with id {userId} does not exist", metadata);
    }
}
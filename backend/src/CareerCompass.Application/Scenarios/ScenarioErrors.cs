using CareerCompass.Application.Common;
using CareerCompass.Application.Fields;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;

namespace CareerCompass.Application.Scenarios;

public static class ScenarioErrorCode
{
    public static string ScenarioErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.Scenario);

    public static class Creation
    {
        private static string _prefix = $"{ScenarioErrorPrefix}.{10}";
        public static string TagNotFound = $"{_prefix}.{20}";
        public static string FieldNotFound = $"{_prefix}.{30}";
        public static string UserNotFound = $"{_prefix}.{40}";
    }

    public static class Read
    {
        private static string _prefix = $"{ScenarioErrorPrefix}.{20}";
        public static string UserNotFound = $"{_prefix}.{10}";
        public static string ScenarioNotFound = $"{_prefix}.{20}";
    }

    public static class Modification
    {
        private static string _prefix = $"{ScenarioErrorPrefix}.{30}";
        public static string TagNotFound = $"{_prefix}.{20}";
        public static string FieldNotFound = $"{_prefix}.{30}";
        public static string UserNotFound = $"{_prefix}.{40}";
    }
}

public static class ScenarioErrors
{
    #region Creation

    public static Error ScenarioCreation_TagNotFound(TagId tagId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "TagId", tagId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Creation Validation: The provided tag id for this scenario does not exist"
            }
        };

        return Error.Validation(ScenarioErrorCode.Creation.TagNotFound,
            $"Tag with id {tagId} does not exist", metadata);
    }


    public static Error ScenarioCreation_FieldNotFound(FieldId fieldId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "FieldId", fieldId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Creation Validation: The provided field id for this scenario does not exist"
            }
        };
        return Error.Validation(ScenarioErrorCode.Creation.FieldNotFound,
            $"Field with id {fieldId} does not exist", metadata);
    }


    public static Error ScenarioCreation_UserNotFound(UserId userId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Creation Validation: The provided user id for this scenario does not exist"
            }
        };
        return Error.Validation(ScenarioErrorCode.Creation.UserNotFound,
            $"User with id {userId} does not exist", metadata);
    }

    #endregion

    #region Modification

    public static Error ScenarioModification_TagNotFound(TagId tagId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "TagId", tagId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Modification Validation: The provided tag id for this scenario does not exist"
            }
        };

        return Error.Validation(ScenarioErrorCode.Modification.TagNotFound,
            $"Tag with id {tagId} does not exist", metadata);
    }


    public static Error ScenarioModification_FieldNotFound(FieldId fieldId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "FieldId", fieldId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Modification Validation: The provided field id for this scenario does not exist"
            }
        };
        return Error.Validation(ScenarioErrorCode.Modification.FieldNotFound,
            $"Field with id {fieldId} does not exist", metadata);
    }


    public static Error ScenarioModification_UserNotFound(UserId userId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Modification Validation: The provided user id for this scenario does not exist"
            }
        };
        return Error.Validation(ScenarioErrorCode.Creation.UserNotFound,
            $"User with id {userId} does not exist", metadata);
    }

    #endregion

    #region Read

    public static Error ScenarioRead_UserNotFound(UserId userId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "UserId", userId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Read Validation: The provided user id for this scenario does not exist"
            }
        };
        return Error.Validation(ScenarioErrorCode.Read.UserNotFound,
            $"User with id {userId} does not exist", metadata);
    }

    #endregion
}
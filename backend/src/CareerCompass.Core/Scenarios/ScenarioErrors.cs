using CareerCompass.Core.Common;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using ErrorOr;

namespace CareerCompass.Core.Scenarios;

public static class ScenarioErrorCode
{
    private static readonly string ScenarioErrorPrefix =
        ApplicationErrorPrefix.Create(ApplicationErrorPrefix.ApplicationErrorType.Scenario);

    public static class Creation
    {
        private static readonly string Prefix = $"{ScenarioErrorPrefix}.{10}";
        public static readonly string TagNotFound = $"{Prefix}.{20}";
        public static readonly string FieldNotFound = $"{Prefix}.{30}";
        public static readonly string UserNotFound = $"{Prefix}.{40}";
        public static readonly string CreationFailed = $"{Prefix}.{50}";
    }

    public static class Read
    {
        private static readonly string Prefix = $"{ScenarioErrorPrefix}.{20}";
        public static readonly string UserNotFound = $"{Prefix}.{10}";
        public static string ScenarioNotFound = $"{Prefix}.{20}";
    }

    public static class Modification
    {
        private static readonly string Prefix = $"{ScenarioErrorPrefix}.{30}";
        public static readonly string ScenarioNotFound = $"{Prefix}.{10}";
        public static readonly string TagNotFound = $"{Prefix}.{20}";
        public static readonly string FieldNotFound = $"{Prefix}.{30}";
        public static readonly string UserNotFound = $"{Prefix}.{40}";
        public static readonly string ModificationFailed = $"{Prefix}.{50}";
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


    public static Error ScenarioCreation_CreationFailed(string scenarioTitle)
    {
        var metadata = new Dictionary<string, object>
        {
            {
                ErrorMetaDataKey.Title,
                "Scenario Creation Validation: The creation of the scenario failed"
            },
            { "ScenarioTitle", scenarioTitle }
        };


        return Error.Failure(ScenarioErrorCode.Creation.CreationFailed,
            $"The creation of the scenario with ScenarioTitle: {scenarioTitle} failed", metadata);
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

    public static Error ScenarioModification_ModificationFailed(string scenarioTitle)
    {
        var metadata = new Dictionary<string, object>
        {
            {
                ErrorMetaDataKey.Title,
                "Scenario Modification Validation: The modification of the scenario failed"
            },
            { "ScenarioTitle", scenarioTitle }
        };

        return Error.Failure(ScenarioErrorCode.Modification.ModificationFailed,
            $"The modification of the scenario with ScenarioTitle: {scenarioTitle} failed", metadata);
    }

    public static Error ScenarioModification_ScenarioNotFound(ScenarioId scenarioId)
    {
        var metadata = new Dictionary<string, object>
        {
            { "ScenarioId", scenarioId.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Modification Validation: The provided scenario id for this scenario does not exist"
            }
        };
        return Error.Validation(ScenarioErrorCode.Modification.ScenarioNotFound,
            $"Scenario with id {scenarioId} does not exist", metadata);
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

    public static Error ScenarioRead_ScenarioNotFound(ScenarioId id)
    {
        var metadata = new Dictionary<string, object>
        {
            { "ScenarioId", id.ToString() },
            {
                ErrorMetaDataKey.Title,
                "Scenario Read: The requested scenario does not exist"
            }
        };
        return Error.NotFound(ScenarioErrorCode.Read.ScenarioNotFound,
            $"Scenario with id {id} does not exist", metadata);
    }
}
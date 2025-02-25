using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Fields;

namespace CareerCompass.Core.Scenarios;

public class ScenarioField : Entity<ScenarioFieldId>
{
    public FieldId FieldId { get; private set; }

    public string Value { get; private set; }


    private ScenarioField(ScenarioFieldId id, FieldId fieldId, string value) : base(id)
    {
        FieldId = fieldId;
        Value = value;
    }

    public void SetValue(string value)
    {
        Value = value;
    }

    public static ScenarioField Create(FieldId fieldId, string value)
    {
        return new(ScenarioFieldId.CreateUnique(), fieldId, value);
    }


#pragma warning disable CS8618, CS9264
    private ScenarioField()
    {
        // Required by EF Core
    }
#pragma warning restore CS8618, CS9264
}
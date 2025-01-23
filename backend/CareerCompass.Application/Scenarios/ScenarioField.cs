using CareerCompass.Application.Fields;

namespace CareerCompass.Application.Scenarios;

public class ScenarioField(FieldId fieldId, string value)
{
    public FieldId FieldId { get; set; } = fieldId;

    public string Value { get; set; } = value;
}
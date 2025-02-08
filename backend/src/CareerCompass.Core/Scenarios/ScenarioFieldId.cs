using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;

namespace CareerCompass.Core.Scenarios;

public class ScenarioFieldId : ValueObject
{
    public Guid Value { get; private set; }


    private ScenarioFieldId(Guid value)
    {
        Value = value;
    }


    public static ScenarioFieldId CreateUnique() => new(Guid.NewGuid());
    public static ScenarioFieldId Create(Guid value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
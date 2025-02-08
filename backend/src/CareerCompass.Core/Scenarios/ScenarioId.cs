using CareerCompass.Core.Common.Models;

namespace CareerCompass.Core.Scenarios;

public class ScenarioId : ValueObject
{
    public Guid Value { get; private set; }


    private ScenarioId(Guid value)
    {
        Value = value;
    }


    public static ScenarioId CreateUnique() => new(Guid.NewGuid());
    public static ScenarioId Create(Guid value) => new(value);
    public static ScenarioId Create(string value) => new(Guid.Parse(value));
    public override string ToString() => Value.ToString();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
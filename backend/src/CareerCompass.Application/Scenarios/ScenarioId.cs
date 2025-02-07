using CareerCompass.Application.Common;

namespace CareerCompass.Application.Scenarios;

public class ScenarioId : EntityId
{
    public ScenarioId(Guid value) : base(value)
    {
    }

    public ScenarioId(string value) : base(value)
    {
    }

    public new static ScenarioId NewId() => new(Guid.CreateVersion7());


    public static implicit operator string(ScenarioId id) => id.Value;
    public static implicit operator ScenarioId(string id) => new(id);
    
}
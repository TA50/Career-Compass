using CareerCompass.Application.Common;

namespace CareerCompass.Application.Scenarios;

public class ScenarioId(Guid value) : EntityId(value)
{
    public new static ScenarioId NewId() => new(Guid.CreateVersion7());
    public static implicit operator Guid(ScenarioId id) => id.Value;

    public static implicit operator ScenarioId(string id) => new(Guid.Parse(id));
    public static implicit operator ScenarioId(Guid id) => new(id);
}
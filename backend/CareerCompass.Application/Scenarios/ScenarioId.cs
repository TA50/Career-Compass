using CareerCompass.Application.Common;

namespace CareerCompass.Application.Scenarios;

public class ScenarioId(Guid value) : EntityId(value)
{
    public static ScenarioId NewId() => new(Guid.NewGuid());
}
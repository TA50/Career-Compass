using CareerCompass.Application.Common;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;

namespace CareerCompass.Application.Scenarios;

public class Scenario(
    ScenarioId id,
    string title,
    ICollection<TagId> tagIds,
    ICollection<ScenarioField> scenarioFields,
    UserId userId,
    DateTime? date
) : Entity<ScenarioId>(id)
{
    public string Title { get; private set; } = title;
    public ICollection<TagId> TagIds { get; private set; } = tagIds;
    public ICollection<ScenarioField> ScenarioFields { get; private set; } = scenarioFields;
    public UserId UserId { get; private set; } = userId;
    public DateTime? Date { get; private set; } = date;
}
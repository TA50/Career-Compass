using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Scenarios;

public class Scenario : AggregateRoot<ScenarioId>
{
    private Scenario(ScenarioId id,
        string title,
        ICollection<TagId> tagIds,
        ICollection<ScenarioField> scenarioFields,
        UserId userId,
        DateTime? date) : base(id)
    {
        _scenarioFields = scenarioFields;
        _tagIds = tagIds;
        UserId = userId;
        Date = date;
        Title = title;
        Created();
    }

    private ICollection<ScenarioField> _scenarioFields;
    private ICollection<TagId> _tagIds;

    public string Title { get; private set; }

    public IReadOnlyList<TagId> TagIds => _tagIds.ToList().AsReadOnly();
    public IReadOnlyList<ScenarioField> ScenarioFields => _scenarioFields.ToList().AsReadOnly();

    public UserId UserId { get; private set; }
    public DateTime? Date { get; private set; }


    public static Scenario Create(string title, UserId userId, DateTime? date)
    {
        return new(ScenarioId.CreateUnique(),
            title,
            new List<TagId>(),
            new List<ScenarioField>(),
            userId,
            date
        );
    }

    public void AddScenarioField(FieldId fieldId, string value)
    {
        var sf = ScenarioField.Create(fieldId, value);
        _scenarioFields.Add(sf);
        Updated();
    }

    public void AddTag(TagId tagId)
    {
        _tagIds.Add(tagId);
        Updated();
    }

#pragma warning disable CS8618
    private Scenario()
    {
        // Required by EF Core
    }
#pragma warning restore CS8618
}
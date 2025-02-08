using System.Collections.Immutable;
using CareerCompass.Core.Scenarios.Commands.CreateScenario;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Api.Contracts.Scenarios;

public class CreateScenarioFieldRequest
{
    public string FieldId { get; set; }
    public string Value { get; set; }

    public CreateScenarioFieldCommand ToScenarioField()
    {
        return new CreateScenarioFieldCommand(CareerCompass.Core.Fields.FieldId.Create(FieldId), Value);
    }
}

public record CreateScenarioRequest
{
    public string Title { get; set; } = string.Empty;
    public IList<string> TagIds { get; set; } = [];

    public DateTime? Date { get; set; }
    public IList<CreateScenarioFieldRequest> ScenarioFields { get; set; } = [];

    public CreateScenarioCommand ToCreateScenarioCommand(UserId userId)
    {
        return new CreateScenarioCommand(Title,
            TagIds.Select(TagId.Create).ToList(),
            ScenarioFields.Select(x => x.ToScenarioField()).ToList(),
            userId,
            Date);
    }
}
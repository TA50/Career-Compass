using System.Collections.Immutable;
using CareerCompass.Application.Scenarios.Commands.CreateScenario;

namespace CareerCompass.Api.Scenarios.Contracts;

public class CreateScenarioFieldRequest
{
    public string FieldId { get; set; }
    public string Value { get; set; }

    public CreateScenarioFieldCommand ToScenarioField()
    {
        return new CreateScenarioFieldCommand(FieldId, Value);
    }
}

public record CreateScenarioRequest
{
    public string Title { get; set; }
    public IList<string> TagIds { get; set; }

    public DateTime? Date { get; set; }
    public IList<CreateScenarioFieldRequest> ScenarioFields { get; set; }

    public CreateScenarioCommand ToCreateScenarioCommand(string userId)
    {
        return new CreateScenarioCommand(Title,
            TagIds.Select(x => x)
                .ToImmutableArray(),
            ScenarioFields.Select(x => x.ToScenarioField()).ToImmutableArray(),
            userId,
            Date);
    }
}
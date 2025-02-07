using System.Collections.Immutable;
using CareerCompass.Application.Scenarios.Commands.UpdateScenario;

namespace CareerCompass.Api.Scenarios.Contracts;

public class UpdateScenarioFieldDto
{
    public string FieldId { get; set; }
    public string Value { get; set; }

    public UpdateScenarioFieldCommand ToScenarioField()
    {
        return new UpdateScenarioFieldCommand(FieldId, Value);
    }
}

public record UpdateScenarioDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public IList<string> TagIds { get; set; }

    public DateTime? Date { get; set; }
    public IList<UpdateScenarioFieldDto> ScenarioFields { get; set; }

    public UpdateScenarioCommand ToUpdateScenarioCommand(string userId)
    {
        return new UpdateScenarioCommand(
            Id: Id,
            Title,
            TagIds.Select(x => x)
                .ToImmutableArray(),
            ScenarioFields.Select(x => x.ToScenarioField()).ToImmutableArray(),
            userId,
            Date);
    }
}
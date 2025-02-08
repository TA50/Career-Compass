using System.Collections.Immutable;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Commands.UpdateScenario;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Api.Contracts.Scenarios;

public class UpdateScenarioFieldDto
{
    public string FieldId { get; set; }
    public string Value { get; set; }

    public UpdateScenarioFieldCommand ToScenarioField()
    {
        return new UpdateScenarioFieldCommand(CareerCompass.Core.Fields.FieldId.Create(FieldId), Value);
    }
}

public record UpdateScenarioDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public IList<string> TagIds { get; set; }

    public DateTime? Date { get; set; }
    public IList<UpdateScenarioFieldDto> ScenarioFields { get; set; }

    public UpdateScenarioCommand ToUpdateScenarioCommand(UserId userId)
    {
        return new UpdateScenarioCommand(
            ScenarioId.Create(Id),
            Title,
            TagIds.Select(TagId.Create)
                .ToList(),
            ScenarioFields.Select(x => x.ToScenarioField()).ToImmutableArray(),
            userId,
            Date);
    }
}
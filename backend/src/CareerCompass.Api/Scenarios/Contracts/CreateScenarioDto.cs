using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.Commands.CreateScenario;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;

namespace CareerCompass.Api.Scenarios.Contracts;

public class CreateScenarioFieldDto
{
    public string FieldId { get; set; }
    public string Value { get; set; }

    public CreateScenarioFieldCommand ToScenarioField()
    {
        return new CreateScenarioFieldCommand(FieldId, Value);
    }
}

public record CreateScenarioDto
{
    public string Title { get; set; }
    public IList<string> TagIds { get; set; }

    public DateTime? Date { get; set; }
    public IList<CreateScenarioFieldDto> ScenarioFields { get; set; }

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
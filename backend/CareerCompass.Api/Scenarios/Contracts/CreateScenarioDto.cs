using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace CareerCompass.Api.Scenarios.Contracts;

public record CreateScenarioFieldDto
{
    public Guid FieldId { get; set; }
    public string Value { get; set; }
}

public record CreateScenarioDto
{
    public string Title { get; set; }
    public IList<Guid> TagIds { get; set; }

    public DateTime? Date { get; set; }
    public IList<CreateScenarioFieldDto> ScenarioFields { get; set; }
}
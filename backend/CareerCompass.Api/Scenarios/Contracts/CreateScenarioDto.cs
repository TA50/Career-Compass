using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace CareerCompass.Api.Scenarios.Contracts;

public record CreateScenarioFieldDto
{
    [Required] public Guid FieldId { get; set; }
    [Required] public string Value { get; set; }
}

public record CreateScenarioDto
{
    [Required] public string Title { get; set; }
    [Required] public Guid UserId { get; set; }
    public IList<Guid> TagIds { get; set; }

    public DateTime? Date { get; set; }
    public IList<CreateScenarioFieldDto> ScenarioFields { get; set; }
}
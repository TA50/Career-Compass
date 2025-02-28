using System.Runtime.InteropServices;
using CareerCompass.Api.Validation;

namespace CareerCompass.Api.Contracts.Scenarios;

public record ListScenarioQuery
{
    public List<Guid>? TagIds { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
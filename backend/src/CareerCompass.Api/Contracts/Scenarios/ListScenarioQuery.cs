namespace CareerCompass.Api.Contracts.Scenarios;

public record ListScenarioQuery
{
    public List<string>? TagIds { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
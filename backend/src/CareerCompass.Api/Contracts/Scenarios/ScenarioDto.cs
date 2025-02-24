namespace CareerCompass.Api.Contracts.Scenarios;

public record ScenarioDto(
    string Id,
    string Title,
    DateTime? Date,
    ICollection<ScenarioFieldDto> ScenarioFields,
    ICollection<string> TagIds
);
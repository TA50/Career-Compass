namespace CareerCompass.Api.Scenarios.Contracts;

public record ScenarioDto(
    string Id,
    string Title,
    ICollection<ScenarioFieldDto> ScenarioFields,
    ICollection<string> TagIds
);
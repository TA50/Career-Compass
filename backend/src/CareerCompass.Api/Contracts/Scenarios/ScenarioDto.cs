namespace CareerCompass.Api.Contracts.Scenarios;

public record ScenarioDto(
    string Id,
    string Title,
    ICollection<ScenarioFieldDto> ScenarioFields,
    ICollection<string> TagIds
);
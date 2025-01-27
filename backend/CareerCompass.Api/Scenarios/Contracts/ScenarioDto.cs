namespace CareerCompass.Api.Scenarios.Contracts;

public record ScenarioDto(
    Guid Id,
    string Title,
    ICollection<ScenarioFieldDto> ScenarioFields,
    ICollection<Guid> TagIds
);


using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Scenarios.Commands.UpdateScenario;

public record UpdateScenarioFieldCommand(string FieldId, string Value);

public record UpdateScenarioCommand(
    string Id,
    string Title,
    ICollection<string> TagIds,
    ICollection<UpdateScenarioFieldCommand> ScenarioFields,
    string UserId,
    DateTime? Date
) : IRequest<ErrorOr<Scenario>>;
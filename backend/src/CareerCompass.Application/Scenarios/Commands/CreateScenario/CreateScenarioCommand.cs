using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Scenarios.Commands.CreateScenario;

public record CreateScenarioFieldCommand(string FieldId, string Value);

public record CreateScenarioCommand(
    string Title,
    ICollection<string> TagIds,
    ICollection<CreateScenarioFieldCommand> ScenarioFields,
    string UserId,
    DateTime? Date
) : IRequest<ErrorOr<Scenario>>;
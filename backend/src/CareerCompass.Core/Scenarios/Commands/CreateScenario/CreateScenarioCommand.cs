using CareerCompass.Core.Fields;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Scenarios.Commands.CreateScenario;

public record CreateScenarioFieldCommand(FieldId FieldId, string Value);

public record CreateScenarioCommand(
    string Title,
    ICollection<TagId> TagIds,
    ICollection<CreateScenarioFieldCommand> ScenarioFields,
    UserId UserId,
    DateTime? Date
) : IRequest<ErrorOr<Scenario>>;
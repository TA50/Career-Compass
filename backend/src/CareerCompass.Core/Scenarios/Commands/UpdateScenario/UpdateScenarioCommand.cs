using CareerCompass.Core.Fields;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Scenarios.Commands.UpdateScenario;

public record UpdateScenarioFieldCommand(FieldId FieldId, string Value);

public record UpdateScenarioCommand(
    ScenarioId Id,
    string Title,
    ICollection<TagId> TagIds,
    ICollection<UpdateScenarioFieldCommand> ScenarioFields,
    UserId UserId,
    DateTime? Date
) : IRequest<ErrorOr<Scenario>>;
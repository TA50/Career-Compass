using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Scenarios.Commands.CreateScenario;

public record CreateScenarioCommand(
    string Title,
    ICollection<TagId> TagIds,
    ICollection<ScenarioField> ScenarioFields,
    UserId UserId,
    DateTime? Date
) : IRequest<ErrorOr<Scenario>>;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Scenarios.UseCases.Contracts;

public record CreateScenarioInput(
    string title,
    ICollection<TagId> tagIds,
    ICollection<ScenarioField> scenarioFields,
    UserId userId,
    DateTime? date
) : IRequest<ErrorOr<Scenario>>;
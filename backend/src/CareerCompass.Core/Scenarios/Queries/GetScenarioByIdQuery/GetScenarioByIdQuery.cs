using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Scenarios.Queries.GetScenarioByIdQuery;

public record GetScenarioByIdQuery(
    ScenarioId Id,
    UserId UserId
) : IRequest<ErrorOr<Scenario>>;
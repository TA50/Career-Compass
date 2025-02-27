using CareerCompass.Core.Users;
using MediatR;

namespace CareerCompass.Core.Scenarios.Commands.Delete;

using ErrorOr;

public record DeleteScenarioCommand(ScenarioId ScenarioId, UserId UserId) : IRequest<ErrorOr<Unit>>;
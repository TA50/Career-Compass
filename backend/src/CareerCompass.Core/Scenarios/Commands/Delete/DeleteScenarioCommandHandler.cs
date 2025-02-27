using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Scenarios.Commands.Delete;

public class DeleteScenarioCommandHandler(
    IScenarioRepository scenarioRepository,
    ILoggerAdapter<DeleteScenarioCommandHandler> logger
) : IRequestHandler<DeleteScenarioCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> Handle(DeleteScenarioCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting scenario with id {ScenarioId}", request.ScenarioId);

        var spec = new GetScenarioByIdSpecification(request.ScenarioId, request.UserId);

        var scenario = await scenarioRepository.Exists(spec, cancellationToken);
        if (!scenario)
        {
            return ScenarioErrors.ScenarioDeletion_ScenarioNotFound(request.ScenarioId);
        }

        var result = await scenarioRepository.Delete(request.ScenarioId, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("Failed to delete scenario with id {ScenarioId}. Reason: {message}", request.ScenarioId,
                result.ErrorMessage ?? "Unknown");
            return ScenarioErrors.ScenarioDeletion_DeletionFailed(request.ScenarioId);
        }

        logger.LogInformation("Scenario with id {ScenarioId} deleted successfully", request.ScenarioId);

        return Unit.Value;
    }
}
using CareerCompass.Core.Common.Abstractions;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Scenarios.Commands.UpdateScenario;

public class UpdateScenarioCommandHandler(
    IScenarioRepository scenarioRepository,
    ITagRepository tagRepository,
    IFieldRepository fieldRepository,
    IUserRepository userRepository,
    ILoggerAdapter<UpdateScenarioCommandHandler> logger
)
    : IRequestHandler<UpdateScenarioCommand, ErrorOr<Scenario>>
{
    public async Task<ErrorOr<Scenario>> Handle(UpdateScenarioCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating scenario {@ScenarioId}", request.Id);


        // Validate Tag Ids: 
        var errors = new List<Error>();
        foreach (var tagId in request.TagIds)
        {
            var tagExists = await tagRepository.Exists(tagId, cancellationToken);
            if (!tagExists)
            {
                errors.Add(ScenarioErrors.ScenarioModification_TagNotFound(tagId));
            }
        }

        // Validate Field Ids:
        foreach (var scenarioField in request.ScenarioFields)
        {
            var fieldExists = await fieldRepository.Exists(scenarioField.FieldId, cancellationToken);
            if (!fieldExists)
            {
                errors.Add(ScenarioErrors.ScenarioModification_FieldNotFound(scenarioField.FieldId));
            }
        }

        var userExists = await userRepository.Exists(request.UserId, cancellationToken);
        if (!userExists)
        {
            errors.Add(ScenarioErrors.ScenarioModification_UserNotFound(request.UserId));
        }

        if (errors.Any())
        {
            return ErrorOr<Scenario>.From(errors);
        }

        var scenario = await scenarioRepository.Get(request.Id, cancellationToken);

        if (scenario is null)
        {
            return ErrorOr<Scenario>.From([
                ScenarioErrors.ScenarioModification_ScenarioNotFound(request.Id)
            ]);
        }

        // update scenario

        var result = await scenarioRepository.Save(cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to update scenario {@Scenario}. Reason: {@message}", scenario,
                result.ErrorMessage ?? "Unknown");

            return ErrorOr<Scenario>.From([
                ScenarioErrors.ScenarioModification_ModificationFailed(request.Title)
            ]);
        }

        logger.LogInformation("Scenario {@ScenarioId} updated successfully", request.Id);
        return scenario;
    }
}
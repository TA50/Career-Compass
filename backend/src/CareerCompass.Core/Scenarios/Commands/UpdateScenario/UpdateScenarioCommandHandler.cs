using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Tags;
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

        var validationResult = await Validate(request, cancellationToken);
        if (validationResult.IsError)
        {
            return validationResult;
        }

        var scenario = validationResult.Value;
        scenario.SetTitle(request.Title);
        scenario.SetDate(request.Date);

        Update(scenario, request.TagIds.ToList());
        Update(scenario, request.ScenarioFields.ToList());
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

    public void Update(Scenario scenario, List<TagId> tagIds)
    {
        var oldTagIds = scenario.TagIds;

        var addedTagIds = tagIds.Except(oldTagIds);
        var removedTagIds = oldTagIds.Except(tagIds);

        foreach (var tagId in addedTagIds)
        {
            scenario.AddTag(tagId);
        }

        foreach (var tagId in removedTagIds)
        {
            scenario.RemoveTag(tagId);
        }
    }

    public void Update(Scenario scenario, List<UpdateScenarioFieldCommand> scenarioFields)
    {
        var inputScenarioFields = scenarioFields.Select(sf => ScenarioField.Create(sf.FieldId, sf.Value)).ToList();
        var addedValues = inputScenarioFields.Except(scenario.ScenarioFields);
        var removedValues = scenario.ScenarioFields.Except(inputScenarioFields);
        var updatedValues = inputScenarioFields.Intersect(scenario.ScenarioFields);

        foreach (var sf in addedValues)
        {
            scenario.AddScenarioField(sf.FieldId, sf.Value);
        }

        foreach (var sf in removedValues)
        {
            scenario.RemoveScenarioField(sf.FieldId);
        }

        foreach (var sf in updatedValues)
        {
            scenario.UpdateScenarioField(sf.FieldId, sf.Value);
        }
    }


    public async Task<ErrorOr<Scenario>> Validate(UpdateScenarioCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();
        foreach (var tagId in request.TagIds)
        {
            var spec = new GetTagByIdSpecification(tagId, request.UserId);
            var tagExists = await tagRepository.Exists(spec, cancellationToken);
            if (!tagExists)
            {
                errors.Add(ScenarioErrors.ScenarioModification_TagNotFound(tagId));
            }
        }

        foreach (var scenarioField in request.ScenarioFields)
        {
            var spec = new GetFieldByIdSpecification(scenarioField.FieldId, request.UserId);
            var fieldExists = await fieldRepository.Exists(spec, cancellationToken);

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
            return errors;
        }

        var scenario = await scenarioRepository.Get(request.Id, true, cancellationToken);
        if (scenario is null)
        {
            return ScenarioErrors.ScenarioModification_ScenarioNotFound(request.Id);
        }

        return scenario;
    }
}
using System.Xml.Schema;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Scenarios.Commands.CreateScenario;

public class CreateScenarioCommandHandler(
    IScenarioRepository scenarioRepository,
    ITagRepository tagRepository,
    IFieldRepository fieldRepository,
    IUserRepository userRepository,
    ILoggerAdapter<CreateScenarioCommandHandler> logger
)
    : IRequestHandler<CreateScenarioCommand, ErrorOr<Scenario>>
{
    public async Task<ErrorOr<Scenario>> Handle(CreateScenarioCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating scenario for user {@UserId} {@ScenarioTitle}", request.UserId, request.Title);
        // Validate User Id:
        var errors = new List<Error>();
        var userExists = await userRepository.Exists(request.UserId, cancellationToken);
        if (!userExists)
        {
            errors.Add(ScenarioErrors.ScenarioCreation_UserNotFound(request.UserId));
        }

        // If User does not exist, return error immediately, we know fields, tags will not exist, no need for extra db calls
        if (errors.Any())
        {
            return ErrorOr<Scenario>.From(errors);
        }

        // Validate Tag Ids: 
        foreach (var tagId in request.TagIds)
        {
            var spec = new GetTagByIdSpecification(tagId, request.UserId);
            var tagExists = await tagRepository.Exists(spec, cancellationToken);
            if (!tagExists)
            {
                errors.Add(ScenarioErrors.ScenarioCreation_TagNotFound(tagId));
            }
        }

        // Validate Field Ids:
        foreach (var scenarioField in request.ScenarioFields)
        {
            var spec = new GetFieldByIdSpecification(scenarioField.FieldId, request.UserId);
            var fieldExists = await fieldRepository.Exists(spec, cancellationToken);
            if (!fieldExists)
            {
                errors.Add(ScenarioErrors.ScenarioCreation_FieldNotFound(scenarioField.FieldId));
            }
        }


        if (errors.Any())
        {
            return ErrorOr<Scenario>.From(errors);
        }

        var scenario = Scenario.Create(
            title: request.Title,
            userId: request.UserId,
            date: request.Date
        );

        foreach (var tagId in request.TagIds)
        {
            scenario.AddTag(tagId);
        }

        foreach (var scenarioField in request.ScenarioFields)
        {
            scenario.AddScenarioField(scenarioField.FieldId, scenarioField.Value);
        }

        var result = await scenarioRepository.Create(scenario, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create scenario with title: {@ScenarioTitle}. Reason: {@message}", scenario.Title,
                result.ErrorMessage ?? "Unknown");
            return ScenarioErrors.ScenarioCreation_CreationFailed(scenario.Title);
        }

        logger.LogInformation("Scenario created successfully with id: {@ScenarioId}", scenario.Id);
        return scenario;
    }
}
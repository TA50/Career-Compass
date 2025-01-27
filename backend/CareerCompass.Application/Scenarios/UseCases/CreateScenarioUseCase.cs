using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios.UseCases.Contracts;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Scenarios.UseCases;

public class CreateScenarioUseCase(
    IScenarioRepository scenarioRepository,
    ITagRepository tagRepository,
    IFieldRepository fieldRepository,
    IUserRepository userRepository
)
    : IRequestHandler<CreateScenarioInput, ErrorOr<Scenario>>
{
    /** Validations
          - Check if user exists.
          - Check if Tags exist.
          - Check if Fields exist (if any).
         */
    public async Task<ErrorOr<Scenario>> Handle(CreateScenarioInput request,
        CancellationToken cancellationToken)
    {
        // Validate Tag Ids: 
        var errors = new List<Error>();
        foreach (var tagId in request.tagIds)
        {
            var tagExists = await tagRepository.Exists(tagId, cancellationToken);
            if (!tagExists)
            {
                errors.Add(ScenarioError.ScenarioValidation_TagNotFound(tagId));
            }
        }

        // Validate Field Ids:
        foreach (var field in request.scenarioFields)
        {
            var fieldExists = await fieldRepository.Exists(field.FieldId, cancellationToken);
            if (!fieldExists)
            {
                errors.Add(ScenarioError.ScenarioValidation_FieldNotFound(field.FieldId));
            }
        }

        var userExists = await userRepository.Exists(request.userId, cancellationToken);
        if (!userExists)
        {
            errors.Add(ScenarioError.ScenarioValidation_UserNotFound(request.userId));
        }

        if (errors.Any())
        {
            return ErrorOr<Scenario>.From(errors);
        }


        var scenario = new Scenario(
            id: ScenarioId.NewId(),
            title: request.title,
            tagIds: request.tagIds,
            scenarioFields: request.scenarioFields,
            userId: request.userId,
            date: request.date
        );

        return await scenarioRepository.Create(scenario, cancellationToken);
    }
}
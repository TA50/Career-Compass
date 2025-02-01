using CareerCompass.Application.Fields;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Scenarios.Commands.CreateScenario;

public class CreateScenarioCommandHandler(
    IScenarioRepository scenarioRepository,
    ITagRepository tagRepository,
    IFieldRepository fieldRepository,
    IUserRepository userRepository
)
    : IRequestHandler<CreateScenarioCommand, ErrorOr<Scenario>>
{
    /** Validations
          - Check if user exists.
          - Check if Tags exist.
          - Check if Fields exist (if any).
         */
    public async Task<ErrorOr<Scenario>> Handle(CreateScenarioCommand request,
        CancellationToken cancellationToken)
    {
        // Validate Tag Ids: 
        var errors = new List<Error>();
        foreach (var tagId in request.TagIds)
        {
            var tagExists = await tagRepository.Exists(tagId, cancellationToken);
            if (!tagExists)
            {
                errors.Add(ScenarioError.ScenarioValidation_TagNotFound(tagId));
            }
        }

        // Validate Field Ids:
        foreach (var field in request.ScenarioFields)
        {
            var fieldExists = await fieldRepository.Exists(field.FieldId, cancellationToken);
            if (!fieldExists)
            {
                errors.Add(ScenarioError.ScenarioValidation_FieldNotFound(field.FieldId));
            }
        }

        var userExists = await userRepository.Exists(request.UserId, cancellationToken);
        if (!userExists)
        {
            errors.Add(ScenarioError.ScenarioValidation_UserNotFound(request.UserId));
        }

        if (errors.Any())
        {
            return ErrorOr<Scenario>.From(errors);
        }


        var scenario = new Scenario(
            id: ScenarioId.NewId(),
            title: request.Title,
            tagIds: request.TagIds,
            scenarioFields: request.ScenarioFields,
            userId: request.UserId,
            date: request.Date
        );

        return await scenarioRepository.Create(scenario, cancellationToken);
    }
}
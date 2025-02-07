using CareerCompass.Application.Fields;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Scenarios.Commands.UpdateScenario;

public class UpdateScenarioCommandHandler(
    IScenarioRepository scenarioRepository,
    ITagRepository tagRepository,
    IFieldRepository fieldRepository,
    IUserRepository userRepository
)
    : IRequestHandler<UpdateScenarioCommand, ErrorOr<Scenario>>
{
    /** Validations
          - Check if user exists.
          - Check if Tags exist.
          - Check if Fields exist (if any).
         */
    public async Task<ErrorOr<Scenario>> Handle(UpdateScenarioCommand request,
        CancellationToken cancellationToken)
    {
        // Validate Tag Ids: 
        var errors = new List<Error>();
        foreach (var tagId in request.TagIds)
        {
            var tagExists = await tagRepository.Exists(new TagId(tagId), cancellationToken);
            if (!tagExists)
            {
                errors.Add(ScenarioError.ScenarioModification_TagNotFound(new TagId(tagId)));
            }
        }

        // Validate Field Ids:
        foreach (var field in request.ScenarioFields)
        {
            var fieldExists = await fieldRepository.Exists(new FieldId(field.FieldId), cancellationToken);
            if (!fieldExists)
            {
                errors.Add(ScenarioError.ScenarioModification_FieldNotFound(new FieldId(field.FieldId)));
            }
        }

        var userExists = await userRepository.Exists(new UserId(request.UserId), cancellationToken);
        if (!userExists)
        {
            errors.Add(ScenarioError.ScenarioModification_UserNotFound(new UserId(request.UserId)));
        }

        if (errors.Any())
        {
            return ErrorOr<Scenario>.From(errors);
        }


        var scenario = new Scenario(
            id: request.Id,
            title: request.Title,
            tagIds: request.TagIds.Select(t => new TagId(t)).ToList(),
            scenarioFields: request.ScenarioFields.Select(
                sf => new ScenarioField(new FieldId(sf.FieldId), sf.Value)
            ).ToList(),
            userId: new UserId(request.UserId),
            date: request.Date
        );

        return await scenarioRepository.Update(scenario, cancellationToken);
    }
}
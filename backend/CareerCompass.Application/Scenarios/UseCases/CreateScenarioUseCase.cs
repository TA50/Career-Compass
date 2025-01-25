using CareerCompass.Application.Scenarios.UseCases.Contracts;
using CareerCompass.Application.Tags;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Scenarios.UseCases;

public class CreateScenarioUseCase(
    IScenarioRepository scenarioRepository,
    ITagRepository tagRepository
)
    : IRequestHandler<CreateScenarioInput, ErrorOr<Scenario>>
{
    /** Assumes that user is authenticated, (userId is valid)
          - Check if Tags exist.
          - Check if Fields exist (if any).
          - Create Scenario.
         */
    public Task<ErrorOr<Scenario>> Handle(CreateScenarioInput request,
        CancellationToken cancellationToken)
    {
        var scenario = new Scenario(
            id: ScenarioId.NewId(),
            title: request.title,
            tagIds: request.tagIds,
            scenarioFields: request.scenarioFields,
            userId: request.userId,
            date: request.date
        );

        return scenarioRepository.Create(scenario, cancellationToken);
    }
}
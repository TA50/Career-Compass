using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Scenarios.Queries.GetScenarioByIdQuery;

public class GetScenarioByIdQueryHandler(
    IScenarioRepository scenarioRepository,
    ILoggerAdapter<GetScenarioByIdQueryHandler> logger) : IRequestHandler<GetScenarioByIdQuery, ErrorOr<Scenario>>
{
    public async Task<ErrorOr<Scenario>> Handle(GetScenarioByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting scenario {ScenarioId} for user {@UserId}", request.Id, request.UserId);
        var spec = new GetScenarioByIdSpecification(request.Id, request.UserId);

        var result = await scenarioRepository.Single(spec, cancellationToken);

        if (result is null)
        {
            return ScenarioErrors.ScenarioRead_ScenarioNotFound(request.Id);
        }

        logger.LogInformation("Got scenario {ScenarioId} for user {@UserId}", request.Id, request.UserId);
        return result;
    }
}
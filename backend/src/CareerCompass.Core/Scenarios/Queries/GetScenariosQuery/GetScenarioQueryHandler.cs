using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;

public class GetScenarioQueryHandler(
    IScenarioRepository scenarioRepository,
    ILoggerAdapter<GetScenarioQueryHandler> logger) : IRequestHandler<GetScenariosQuery, ErrorOr<IList<Scenario>>>
{
    public async Task<ErrorOr<IList<Scenario>>> Handle(GetScenariosQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting scenarios for user {@UserId}", request.UserId);
        var spec = new GetUserScenariosSpecification(request.UserId);

        var result = await scenarioRepository.Get(spec, cancellationToken);

        logger.LogInformation("Found {ScenariosCount} scenarios for user {@UserId}", result.Count, request.UserId);
        return ErrorOrFactory.From(result);
    }
}
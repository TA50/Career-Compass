using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Scenarios.Queries.GetScenariosQuery;

public class GetScenarioQueryHandler(
    IScenarioRepository scenarioRepository
) : IRequestHandler<GetScenariosQuery, ErrorOr<IList<Scenario>>>
{
    public async Task<ErrorOr<IList<Scenario>>> Handle(GetScenariosQuery request, CancellationToken cancellationToken)
    {
        // We don't need to check if the user exists, if the user doesn't exist, the repository will return an empty list anyway
        var result = await scenarioRepository.Get(new UserId(request.UserId), cancellationToken);

        return ErrorOrFactory.From(result);
    }
}
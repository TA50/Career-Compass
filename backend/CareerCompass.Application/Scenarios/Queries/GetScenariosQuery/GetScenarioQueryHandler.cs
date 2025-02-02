using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Scenarios.Queries.GetScenariosQuery;

public class GetScenarioQueryHandler(
    IScenarioRepository scenarioRepository,
    IUserRepository userRepository
) : IRequestHandler<GetScenariosQuery, ErrorOr<IList<Scenario>>>
{
    public async Task<ErrorOr<IList<Scenario>>> Handle(GetScenariosQuery request, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();
        var userExists = await userRepository.Exists(new UserId(request.UserId), cancellationToken);
        if (!userExists)
        {
            errors.Add(ScenarioError.ScenarioCreation_UserNotFound(new UserId(request.UserId)));
        }

        if (errors.Any())
        {
            return ErrorOr<IList<Scenario>>.From(errors);
        }

        var result = await scenarioRepository.Get(new UserId(request.UserId), cancellationToken);

        return ErrorOrFactory.From(result);
    }
}
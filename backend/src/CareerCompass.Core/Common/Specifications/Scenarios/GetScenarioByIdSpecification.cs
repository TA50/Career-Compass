using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Scenarios;

public class GetScenarioByIdSpecification(ScenarioId scenarioId, UserId userId)
    : EquatableModel<GetScenariosSpecification>, ISpecification<Scenario, ScenarioId>
{
    public IQueryable<Scenario> Apply(IQueryable<Scenario> query)
    {
        return query
            .Where(s => s.UserId == userId && s.Id == scenarioId);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return userId;
        yield return scenarioId;
    }
}
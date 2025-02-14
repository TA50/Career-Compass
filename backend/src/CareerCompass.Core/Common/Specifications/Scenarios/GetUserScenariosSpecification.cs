using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Scenarios;

public class GetUserScenariosSpecification(UserId userId)
    : EquatableModel<GetUserScenariosSpecification>, ISpecification<Scenario, ScenarioId>
{
    public IQueryable<Scenario> Apply(IQueryable<Scenario> query)
    {
        return query
            .Where(s => s.UserId == userId);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return userId;
    }
}
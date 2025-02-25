using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;

namespace CareerCompass.Core.Common.Specifications.Scenarios;

public class GetScenarioHavingTagsSpecification(IList<TagId> tagIds)
    : EquatableModel<GetScenarioHavingTagsSpecification>, ISpecification<Scenario, ScenarioId>
{
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return tagIds;
    }

    public IQueryable<Scenario> Apply(IQueryable<Scenario> query)
    {
        return query
            .Where(scenario =>
                scenario.TagIds.Any(x => tagIds
                    .Select(tagId => tagId.Value)
                    .Contains(x.Value)
                )
            );
    }
}
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;

namespace CareerCompass.Core.Common.Specifications.Scenarios;

public class GetScenarioHavingField(FieldId fieldId)
    : EquatableModel<GetScenarioHavingField>, ISpecification<Scenario, ScenarioId>
{
    public IQueryable<Scenario> Apply(IQueryable<Scenario> query)
    {
     return query
         .Where( s=> s.ScenarioFields.Any(sf=> sf.FieldId == fieldId));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return fieldId;
    }
}
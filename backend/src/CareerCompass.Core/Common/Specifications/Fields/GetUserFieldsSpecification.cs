using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Fields;

public class GetUserFieldsSpecification(UserId userId)
    : EquatableModel<GetUserFieldsSpecification>, ISpecification<Field, FieldId>
{
    public IQueryable<Field> Apply(IQueryable<Field> query)
    {
        return query
            .Where(f => f.UserId == userId);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return userId;
    }
}
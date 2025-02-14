using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Fields;

public class GetFieldByIdSpecification(FieldId id, UserId userId)
    : EquatableModel<GetFieldByIdSpecification>, ISpecification<Field, FieldId>
{
    public IQueryable<Field> Apply(IQueryable<Field> query)
    {
        return query
            .Where(f => f.UserId == userId)
            .Where(f => f.Id == id);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return id;
        yield return userId;
    }
}
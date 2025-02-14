using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Fields;

public class GetUserFieldByNameAndGroupSpecification(UserId userId, string name, string group)
    : EquatableModel<GetUserFieldByNameAndGroupSpecification>, ISpecification<Field, FieldId>
{
    public IQueryable<Field> Apply(IQueryable<Field> query)
    {
        return query
            .Where(f => f.UserId == userId)
            .Where(f => f.Name == name)
            .Where(f => f.Group == group);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return userId;
        yield return name;
        yield return group;
    }
}
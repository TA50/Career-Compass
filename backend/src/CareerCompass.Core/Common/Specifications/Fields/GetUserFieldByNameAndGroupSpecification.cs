using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Fields;

public class GetUserFieldByNameAndGroupSpecification(UserId userId, string name, string group)
    : ISpecification<Field, FieldId>
{
    public IQueryable<Field> Apply(IQueryable<Field> query)
    {
        return query
            .Where(f => f.UserId == userId)
            .Where(f => f.Name == name)
            .Where(f => f.Group == group);
    }
}
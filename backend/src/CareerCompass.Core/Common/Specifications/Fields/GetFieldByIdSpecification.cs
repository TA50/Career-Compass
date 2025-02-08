using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Fields;

public class GetFieldByIdSpecification(FieldId id, UserId userId) : ISpecification<Field, FieldId>
{
    public IQueryable<Field> Apply(IQueryable<Field> query)
    {
        return query
            .Where(f => f.UserId == userId)
            .Where(f => f.Id == id);
    }
}
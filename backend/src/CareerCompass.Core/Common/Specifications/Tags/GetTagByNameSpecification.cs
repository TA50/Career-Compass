using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Tags;

public class GetTagByNameSpecification(UserId userId, string name)
    : EquatableModel<GetTagByNameSpecification>, ISpecification<Tag, TagId>
{
    public IQueryable<Tag> Apply(IQueryable<Tag> query)
    {
        return query
            .Where(t => t.UserId == userId)
            .Where(t => t.Name == name);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return userId;
        yield return name;
    }
}
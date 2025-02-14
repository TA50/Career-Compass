using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Tags;

public class GetUserTagsSpecification(UserId userId)
    : EquatableModel<GetUserTagsSpecification>, ISpecification<Tag, TagId>
{
    public IQueryable<Tag> Apply(IQueryable<Tag> query)
    {
        return query
            .Where(t => t.UserId == userId);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return userId;
    }
}
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Tags;

public class GetTagByIdSpecification(TagId id, UserId userId)
    : EquatableModel<GetTagByIdSpecification>, ISpecification<Tag, TagId>
{
    public IQueryable<Tag> Apply(IQueryable<Tag> query)
    {
        return query
            .Where(t => t.UserId == userId)
            .Where(t => t.Id == id);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return id;
        yield return userId;
    }
}
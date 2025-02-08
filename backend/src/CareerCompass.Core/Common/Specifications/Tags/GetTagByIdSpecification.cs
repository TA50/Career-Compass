using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Tags;

public class GetTagByIdSpecification(TagId id, UserId userId) : ISpecification<Tag, TagId>
{
    public IQueryable<Tag> Apply(IQueryable<Tag> query)
    {
        return query
            .Where(t => t.UserId == userId)
            .Where(t => t.Id == id);
    }
}
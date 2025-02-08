using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Users;

public class GetUserByIdSpecification(UserId userId) : ISpecification<User, UserId>
{
    public IQueryable<User> Apply(IQueryable<User> query)
    {
        return query
            .Where(u => u.Id == userId);
    }
}
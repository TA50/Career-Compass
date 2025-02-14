using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Users;

public class UserWithEmailSpecification(string email)
    : EquatableModel<UserWithEmailSpecification>, ISpecification<User, UserId>
{
    public IQueryable<User> Apply(IQueryable<User> query)
    {
        return query.Where(x => x.Email == email);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return email;
    }
}
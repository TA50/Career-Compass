using CareerCompass.Core.Users;

namespace CareerCompass.Core.Common.Specifications.Users;

public class GetUserByEmailSpecification(string email)
    : EquatableModel<GetUserByEmailSpecification>, ISpecification<User, UserId>
{
    private bool _confirmationRequired;
    private UserId? _userId;

    public IQueryable<User> Apply(IQueryable<User> query)
    {
        var q = query.Where(x => x.Email == email);

        if (_userId is not null)
        {
            q = q.Where(x => x.Id != _userId);
        }

        if (_confirmationRequired)
        {
            q = q.Where(x => x.EmailConfirmed);
        }

        return q;
    }

    public GetUserByEmailSpecification RequireConfirmation()
    {
        _confirmationRequired = true;
        return this;
    }

    public GetUserByEmailSpecification ExcludeUser(UserId userId)
    {
        _userId = userId;
        return this;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return email;
    }
}
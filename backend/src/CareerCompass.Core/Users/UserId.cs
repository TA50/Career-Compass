using CareerCompass.Core.Common.Models;

namespace CareerCompass.Core.Users;

public class UserId : ValueObject, IEquatable<UserId>
{
    public Guid Value { get; private set; }


    private UserId(Guid value)
    {
        Value = value;
    }


    public static UserId CreateUnique() => new(Guid.NewGuid());
    public static UserId Create(Guid value) => new(value);
    public static UserId Create(string value) => new(Guid.Parse(value));

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public bool Equals(UserId? other)
    {
        return base.Equals(other);
    }
}
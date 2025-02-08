namespace CareerCompass.Core.Common.Models;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }


    public static bool operator ==(ValueObject one, ValueObject two)
    {
        return one.Equals(two);
    }

    public static bool operator !=(ValueObject one, ValueObject two)
    {
        return !one.Equals(two);
    }
}
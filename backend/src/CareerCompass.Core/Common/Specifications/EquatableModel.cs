namespace CareerCompass.Core.Common.Specifications;

public abstract class EquatableModel<T> : IEquatable<T>
    where T : class
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    #region IEquatable

    public bool Equals(T? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (EquatableModel<T>)obj;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }


    public static bool operator ==(EquatableModel<T> one, EquatableModel<T> two)
    {
        return one.Equals(two);
    }

    public static bool operator !=(EquatableModel<T> one, EquatableModel<T> two)
    {
        return !one.Equals(two);
    }

    #endregion
}
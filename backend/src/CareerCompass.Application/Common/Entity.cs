namespace CareerCompass.Application.Common;

public abstract class Entity<TId>(TId id)
    where TId : EntityId
{
    public TId Id { get; init; } = id;

    protected virtual IEnumerable<object?> GetEqualityComponents()
    {
        return [Id, typeof(TId).Name];
    }

    protected static bool EqualOperator(Entity<TId> left, Entity<TId> right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }

        return ReferenceEquals(left, right) || left!.Equals(right);
    }

    protected static bool NotEqualOperator(Entity<TId> left, Entity<TId> right)
    {
        return !(EqualOperator(left, right));
    }


    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (Entity<TId>)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(Entity<TId> one, Entity<TId> two)
    {
        return EqualOperator(one, two);
    }

    public static bool operator !=(Entity<TId> one, Entity<TId> two)
    {
        return NotEqualOperator(one, two);
    }
}
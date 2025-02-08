namespace CareerCompass.Core.Common.Models;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; private set; }


    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && entity.Id.Equals(Id);
    }


    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId> one, Entity<TId> two)
    {
        return Equals(one, two);
    }

    public static bool operator !=(Entity<TId> one, Entity<TId> two)
    {
        return !Equals(one, two);
    }


#pragma warning disable CS8618
    protected Entity()
    {
        // Required by EF Core
    }
#pragma warning restore CS8618
}
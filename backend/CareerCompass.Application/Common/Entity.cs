namespace CareerCompass.Application.Common;

public abstract class Entity<TId>(TId id)
    where TId : ValueObject
{
    public TId Id { get; init; } = id;
}
namespace CareerCompass.Application.Common;

public abstract class Entity<TId>(TId id)
    where TId : ValueObject
{
    public required TId Id { get; set; } = id;
}
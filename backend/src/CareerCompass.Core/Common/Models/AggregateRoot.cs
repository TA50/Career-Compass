using CareerCompass.Core.Common.Abstractions;

namespace CareerCompass.Core.Common.Models;

public abstract class AggregateRoot<TId> : Entity<TId>, IAuditable
    where TId : ValueObject
{
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
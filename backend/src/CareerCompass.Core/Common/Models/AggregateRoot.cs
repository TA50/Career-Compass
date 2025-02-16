using System.ComponentModel;
using System.Runtime.CompilerServices;
using CareerCompass.Core.Common.Abstractions;

namespace CareerCompass.Core.Common.Models;

public abstract class AggregateRoot<TId> : Entity<TId>, IAuditable
    where TId : ValueObject
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    protected void Updated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    protected void Created()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
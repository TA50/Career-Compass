namespace CareerCompass.Application.Common;

public class EntityId(Guid value) : ValueObject
{
    public Guid Value { get; } = value;

    public static EntityId NewId() => new(Guid.NewGuid());

    public static implicit operator Guid(EntityId id) => id.Value;

    public static implicit operator EntityId(Guid id) => new(id);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return
        [
            Value,
        ];
    }
}
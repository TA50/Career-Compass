namespace CareerCompass.Application.Common;

public class EntityId : ValueObject
{
    public EntityId(Guid value) => Value = value;

    protected EntityId()
    {
    }

    public Guid Value { get; protected set; }
    public override string ToString() => Value.ToString();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return
        [
            Value,
        ];
    }

    public EntityId From<T>(string value) where T : EntityId, new()
    {
        var id = new T();
        id.Value = Guid.Parse(value);
        return id;
    }

    public EntityId From<T>(Guid value) where T : EntityId, new()
    {
        var id = new T();
        id.Value = value;
        return id;
    }


    public static EntityId NewId() => new(Guid.CreateVersion7());
}
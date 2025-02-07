using ErrorOr;

namespace CareerCompass.Application.Common;

public class EntityId : ValueObject
{
    public EntityId(Guid _guid)
    {
        Value = _guid.ToString();
    }

    public EntityId(string _str)
    {
        Value = _str;
    }


    public string Value { get; protected set; }
    public override string ToString() => Value.ToString();


    public static ErrorOr<bool> Validate(string id)
    {
        if (Guid.TryParse(id, out _))
        {
            return true;
        }

        return Error.Failure("Invalid Guid Value", $"Value {id} is not a valid Guid");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return
        [
            Value,
        ];
    }


    public static EntityId NewId() => new(Guid.CreateVersion7());
}
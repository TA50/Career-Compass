using CareerCompass.Application.Common;

namespace CareerCompass.Application.Fields;

public class FieldId : EntityId
{
    public FieldId(Guid value) : base(value)
    {
    }

    public FieldId(string value) : base(value)
    {
    }

    public new static FieldId NewId() => new(Guid.CreateVersion7());

    public static implicit operator string(FieldId id) => id.Value;
    public static implicit operator FieldId(string id) => new(id);
}
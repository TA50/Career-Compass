using CareerCompass.Application.Common;

namespace CareerCompass.Application.Fields;

public class FieldId(Guid value) : EntityId(value)
{
    public new static FieldId NewId() => new(Guid.CreateVersion7());
    public static implicit operator Guid(FieldId id) => id.Value;

    public static implicit operator FieldId(string id) => new(Guid.Parse(id));
    public static implicit operator FieldId(Guid id) => new(id);
}
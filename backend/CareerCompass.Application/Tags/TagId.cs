using CareerCompass.Application.Common;

namespace CareerCompass.Application.Tags;

public class TagId(Guid value) : EntityId(value)
{
    public new static TagId NewId() => new(Guid.CreateVersion7());
    public static implicit operator Guid(TagId id) => id.Value;

    public static implicit operator TagId(string id) => new(Guid.Parse(id));
    public static implicit operator TagId(Guid id) => new(id);
}
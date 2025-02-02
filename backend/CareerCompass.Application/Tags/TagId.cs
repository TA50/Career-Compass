using CareerCompass.Application.Common;
using ErrorOr;

namespace CareerCompass.Application.Tags;

public class TagId : EntityId
{
    public TagId(Guid value) : base(value)
    {
    }

    public TagId(string value) : base(value)
    {
    }

    public new static TagId NewId() => new(Guid.CreateVersion7());
    public static implicit operator string(TagId id) => id.Value.ToString();

    public static implicit operator TagId(Guid id) => new(id);
    public static implicit operator TagId(string id) => new(id);
}
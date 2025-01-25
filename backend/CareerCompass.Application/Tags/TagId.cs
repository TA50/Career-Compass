using CareerCompass.Application.Common;

namespace CareerCompass.Application.Tags;

public class TagId(Guid value) : EntityId(value)
{
    public static TagId NewId() => new(Guid.NewGuid());
}
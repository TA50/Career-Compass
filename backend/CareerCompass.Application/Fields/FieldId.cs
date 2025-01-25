using CareerCompass.Application.Common;

namespace CareerCompass.Application.Fields;

public class FieldId(Guid value) : EntityId(value)
{
    public static FieldId NewId() => new(Guid.NewGuid());
}
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;

namespace CareerCompass.Core.Fields;

public class FieldId : ValueObject
{
    public Guid Value { get; private set; }


    private FieldId(Guid value)
    {
        Value = value;
    }


    public static FieldId CreateUnique() => new(Guid.NewGuid());
    public static FieldId Create(Guid value) => new(value);
    public static FieldId Create(string value) => new(Guid.Parse(value));
    public override string ToString() => Value.ToString();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
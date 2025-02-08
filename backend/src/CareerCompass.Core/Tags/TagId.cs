using CareerCompass.Core.Common.Models;

namespace CareerCompass.Core.Tags;

public class TagId : ValueObject
{
    public Guid Value { get; private set; }


    private TagId(Guid value)
    {
        Value = value;
    }


    public static TagId CreateUnique() => new(Guid.NewGuid());
    public static TagId Create(Guid value) => new(value);
    public static TagId Create(string value) => new(Guid.Parse(value));
    public override string ToString() => Value.ToString();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
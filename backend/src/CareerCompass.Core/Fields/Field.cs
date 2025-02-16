using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Fields;

public sealed class Field : AggregateRoot<FieldId>
{
    public string Name { get; private set; }

    public string Group { get; private set; }
    public UserId UserId { get; private set; }


    private Field(FieldId id,
        UserId userId,
        string name,
        string group) : base(id)

    {
        UserId = userId;
        Name = name;
        Group = group;
        Created();
    }

    public static Field Create(UserId userId, string name, string group)
    {
        return new(FieldId.CreateUnique(), userId, name, group);
    }

    private Field()
    {
        // Required by EF Core
    }
}
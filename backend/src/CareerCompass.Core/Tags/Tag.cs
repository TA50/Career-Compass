using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Users;

namespace CareerCompass.Core.Tags;

public sealed class Tag : AggregateRoot<TagId>
{
    public string Name { get; private set; }

    public UserId UserId { get; private set; }


    private Tag(TagId id,
        UserId userId,
        string name
    ) : base(id)

    {
        UserId = userId;
        Name = name;
        Created();
    }

    public static Tag Create(UserId userId, string name)
    {
        return new(TagId.CreateUnique(), userId, name);
    }

    private Tag()
    {
        // Required by EF Core
    }
}
using CareerCompass.Application.Common;

namespace CareerCompass.Application.Users;

public class UserId(Guid value) : EntityId(value)
{
    public new static UserId NewId() => new(Guid.CreateVersion7());
    public static implicit operator Guid(UserId id) => id.Value;

    public static implicit operator UserId(string id) => new(Guid.Parse(id));
    public static implicit operator UserId(Guid id) => new(id);
}
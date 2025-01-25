using CareerCompass.Application.Common;

namespace CareerCompass.Application.Users;

public class UserId(Guid value) : EntityId(value)
{
    public static UserId NewId() => new(Guid.NewGuid());
}
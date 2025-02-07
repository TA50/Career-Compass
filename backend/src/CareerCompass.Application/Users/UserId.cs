using System.Runtime.InteropServices.JavaScript;
using CareerCompass.Application.Common;
using ErrorOr;

namespace CareerCompass.Application.Users;

public class UserId : EntityId
{
    public UserId(Guid value) : base(value)
    {
    }

    public UserId(string value) : base(value)
    {
    }

    public new static UserId NewId() => new(Guid.CreateVersion7());
    public static implicit operator string(UserId id) => id.Value;
    public static implicit operator UserId(string id) => new(id);
}
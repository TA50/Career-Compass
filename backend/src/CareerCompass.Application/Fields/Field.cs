using CareerCompass.Application.Common;
using CareerCompass.Application.Users;

namespace CareerCompass.Application.Fields;

public class Field(FieldId id, string name, UserId userId) : Entity<FieldId>(id)
{
    public string Name { get; set; } = name;
    public UserId UserId { get; set; } = userId;
}
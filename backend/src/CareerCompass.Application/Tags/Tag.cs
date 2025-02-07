using CareerCompass.Application.Common;
using CareerCompass.Application.Users;

namespace CareerCompass.Application.Tags;

public class Tag(TagId id, string name, UserId userId) : Entity<TagId>(id)
{
    public string Name { get; set; } = name;
    public UserId UserId { get; set; } = userId;
}
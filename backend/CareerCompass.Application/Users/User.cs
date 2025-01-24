using CareerCompass.Application.Common;
using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Tags;

namespace CareerCompass.Application.Users;

public class User(
    UserId id,
    string email,
    string firstName,
    string lastName,
    ICollection<TagId> tagIds,
    ICollection<FieldId> fieldIds,
    ICollection<ScenarioId> scenarioIds)
    : Entity<UserId>(id)
{
    public string Email { get; private set; } = email;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;

    public ICollection<TagId> TagIds { get; private set; } = tagIds;
    public ICollection<FieldId> FieldIds { get; private set; } = fieldIds;
    public ICollection<ScenarioId> ScenarioIds { get; private set; } = scenarioIds;
    
}
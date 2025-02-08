using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;

namespace CareerCompass.Core.Users;

public class User : Entity<UserId>
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private User(UserId id, string email, string firstName, string lastName) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public static User Create(string email, string firstName, string lastName)
    {
        return new(UserId.CreateUnique(), email, firstName, lastName);
    }

    public void SetName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
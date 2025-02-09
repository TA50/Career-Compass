using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;

namespace CareerCompass.Core.Users;

public class User : Entity<UserId>
{
    public string UserName { get; private set; }

    public string Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    private User(UserId id,
        string userName,
        string password,
        string firstName, string lastName) : base(id)
    {
        UserName = userName;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
    }

    public static User Create(string username, string password, string firstName, string lastName)
    {
        return new(UserId.CreateUnique(), username, password, firstName, lastName);
    }

    public void SetName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}
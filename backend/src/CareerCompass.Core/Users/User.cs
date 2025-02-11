using System.Text;
using System.Text.RegularExpressions;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;

namespace CareerCompass.Core.Users;

public class User : AggregateRoot<UserId>
{
    public string Email { get; private set; }
    public string Password { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public bool EmailConfirmed { get; private set; }
    public string? EmailConfirmationCode { get; private set; }

    private User(
        UserId id,
        string email,
        string password,
        string firstName,
        string lastName,
        bool emailConfirmed,
        string? emailConfirmationCode) : base(id)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        EmailConfirmed = emailConfirmed;
        EmailConfirmationCode = emailConfirmationCode;
    }

    public static User Create(
        string email,
        string password,
        string firstName,
        string lastName)
    {
        return new User(
            id: UserId.CreateUnique(),
            email: email,
            password: password,
            firstName: firstName,
            lastName: lastName,
            emailConfirmed: false,
            emailConfirmationCode: null);
    }

    public void SetName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public void SetPassword(string password)
    {
        Password = password;
    }

    public void UpdateEmail(string email)
    {
        if (email == Email)
        {
            return;
        }

        Email = email;
        EmailConfirmed = false;
    }

    public bool ConfirmEmail(string code)
    {
        var codesDontMatch = string.IsNullOrEmpty(EmailConfirmationCode)
                             || string.IsNullOrEmpty(code)
                             || EmailConfirmationCode != code;

        if (codesDontMatch)
        {
            return false;
        }


        EmailConfirmed = true;
        EmailConfirmationCode = null;

        return true;
    }


    /// <summary>
    /// Sets a new email confirmation code and returns it
    /// </summary>
    /// <returns>EmailConfirmationCode</returns>
    public string GenerateEmailConfirmationCode()
    {
        var random = new Random();
        const string chars = "0123456789";

        var stringBuild = new StringBuilder();

        foreach (var chrs in Enumerable.Repeat(chars, Limits.EmailConfirmationCodeLength))
        {
            var next = random.Next(chrs.Length);
            var c = chrs[next];

            stringBuild.Append(c);
        }

        EmailConfirmationCode = stringBuild.ToString();

        return EmailConfirmationCode;
    }
}
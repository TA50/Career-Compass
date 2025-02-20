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

    public DateTime? EmailConfirmationCodeExpireAt { get; private set; }
    public string? ForgotPasswordCode { get; private set; }

    public DateTime? ForgotPasswordCodeExpireAt { get; private set; }

    private User(
        UserId id,
        string email,
        string password,
        string firstName,
        string lastName) : base(id)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        EmailConfirmed = false;
        EmailConfirmationCode = null;
        ForgotPasswordCode = null;
        Created();
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
            lastName: lastName
        );
    }

    public void SetName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        Updated();
    }

    public void SetPassword(string password)
    {
        Password = password;
        ForgotPasswordCode = null;
        Updated();
    }

    public void ChangeEmail(string email)
    {
        if (email == Email)
        {
            return;
        }

        Email = email;
        EmailConfirmed = false;
        Updated();
    }

    public bool ConfirmEmail(string code)
    {
        if (!IsCodeValid(EmailConfirmationCode, code, EmailConfirmationCodeExpireAt))
        {
            return false;
        }

        EmailConfirmed = true;
        EmailConfirmationCode = null;
        EmailConfirmationCodeExpireAt = null;
        Updated();
        return true;
    }


    /// <summary>
    /// Sets a new email confirmation code and returns it
    /// </summary>
    /// <returns>EmailConfirmationCode</returns>
    public string GenerateEmailConfirmationCode(TimeSpan expiresAfter)
    {
        EmailConfirmationCode = GenerateCode(length: Limits.EmailConfirmationCodeLength);
        EmailConfirmationCodeExpireAt = DateTime.Now.Add(expiresAfter);

        Updated();
        return EmailConfirmationCode;
    }


    public string GenerateForgotPasswordCode(TimeSpan expiresAfter)
    {
        ForgotPasswordCode = GenerateCode(length: Limits.ForgotPasswordCodeLength);
        ForgotPasswordCodeExpireAt = DateTime.Now.Add(expiresAfter);

        Updated();
        return ForgotPasswordCode;
    }

    public bool ConfirmForgotPassword(string code)
    {
        if (!IsCodeValid(code, ForgotPasswordCode, ForgotPasswordCodeExpireAt))
        {
            return false;
        }

        ForgotPasswordCode = null;
        ForgotPasswordCodeExpireAt = null;
        Updated();

        return true;
    }


    private bool IsCodeValid(string? actual, string? expected, DateTime? expiresAt)
    {
        if (actual is null || expiresAt is null) return false;

        if (!DoesCodeMatch(actual, expected))
        {
            return false;
        }

        if (DateTime.Now > expiresAt)
        {
            return false;
        }

        return true;
    }

    private bool DoesCodeMatch(string? actual, string? expectedCode)
    {
        return string.IsNullOrEmpty(expectedCode)
               || string.IsNullOrEmpty(actual)
               || expectedCode == actual;
    }

    private string GenerateCode(int length)
    {
        var random = new Random();
        const string chars = "0123456789";

        var stringBuild = new StringBuilder();

        foreach (var chrs in Enumerable.Repeat(chars, length))
        {
            var next = random.Next(chrs.Length);
            var c = chrs[next];

            stringBuild.Append(c);
        }

        return stringBuild.ToString();
    }
}
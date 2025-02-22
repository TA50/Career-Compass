using CareerCompass.Core.Common.Abstractions.Email;

namespace CareerCompass.Api.Emails;

public class ForgotPasswordEmail(string from, string to, string code, int codeLifetimeInHours) : IMail
{
    public string Subject => "Change your password";
    public IReadOnlyList<string> Recipients { get; } = new[] { to }.AsReadOnly();
    public string Sender => from;

    public MailBody Body => new()
    {
        IsHtml = false,
        Text = $@"
            You have requested to change your password. Use the code below to change your password. 
            {code}.

            This code will expire in {codeLifetimeInHours} hours.

            If you did not request to change your password, please ignore this email.
            "
    };
}
using CareerCompass.Core.Common.Abstractions.Email;

namespace CareerCompass.Api.Emails;

public class EmailConfirmationEmail(string from, string to, string code, int codeLifetimeInHours)
    : IMail
{
    public string Subject { get; private set; } = "Welcome to Career Compass";
    public IReadOnlyList<string> Recipients { get; private set; } = new List<string> { to }.AsReadOnly();
    public string Sender { get; private set; } = from;

    public MailBody Body { get; private set; } = new()
    {
        IsHtml = false,
        Text = $@"
            Welcome to Career Compass!.

            Please confirm your email using the code below.
            {code}.

            This code will expire in {codeLifetimeInHours} hours.


            If you did not register, please ignore this email.
            "
    };
}
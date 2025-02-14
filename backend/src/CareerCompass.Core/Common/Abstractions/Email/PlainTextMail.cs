namespace CareerCompass.Core.Common.Abstractions.Email;

public class PlainTextMail : IMail
{
    private readonly List<string> _recipients = [];

    public PlainTextMail(
        string sender,
        string recipient
    )
    {
        if (string.IsNullOrEmpty(sender.Trim()))
        {
            throw new ArgumentException("Sender cannot be null or empty", nameof(sender));
        }

        if (string.IsNullOrEmpty(recipient.Trim()))
        {
            throw new ArgumentException("Recipient cannot be null or empty", nameof(recipient));
        }

        Sender = sender;
        _recipients.Add(recipient);
        Subject = string.Empty;
        Body = default;
    }

    public PlainTextMail WithSubject(string subject)
    {
        Subject = subject;
        return this;
    }

    public PlainTextMail WithRecipient(string recipient)
    {
        _recipients.Add(recipient);
        return this;
    }

    public PlainTextMail WithBody(string body)
    {
        Body = new MailBody
        {
            Text = body,
            IsHtml = false
        };
        return this;
    }

    public string Subject { get; private set; }
    public IReadOnlyList<string> Recipients => _recipients.AsReadOnly();
    public string Sender { get; }
    public MailBody Body { get; private set; }
}
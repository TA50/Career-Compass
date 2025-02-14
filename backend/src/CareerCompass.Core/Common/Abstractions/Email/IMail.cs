namespace CareerCompass.Core.Common.Abstractions.Email;

public struct MailBody
{
    public string Text { get; set; }
    public bool IsHtml { get; set; }
}

public interface IMail
{
    public string Subject { get; }
    public IReadOnlyList<string> Recipients { get; }
    public string Sender { get; }
    public MailBody Body { get; }
}
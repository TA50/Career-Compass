using System.Net.Mail;
using CareerCompass.Core.Common.Abstractions.Email;

namespace CareerCompass.Infrastructure.Services;

public static class MailExtensions
{
    public static MailMessage ToMailMessage(this IMail mail)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(mail.Sender),
            Subject = mail.Subject,
            Body = mail.Body.Text,
            IsBodyHtml = mail.Body.IsHtml
        };
        foreach (var i in mail.Recipients)
        {
            mailMessage.To.Add(i);
        }

        return mailMessage;
    }
}
using System.Net;
using System.Net.Mail;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace CareerCompass.Infrastructure.Services;

internal class MailHogEmailService : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;

    public MailHogEmailService(IOptions<SmtpSettings> options)
    {
        _smtpSettings = options.Value;
    }

    public async Task Send(string email, string subject, string message)
    {
        using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
        {
            client.EnableSsl = _smtpSettings.EnableSsl;
            client.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("noreply@example.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
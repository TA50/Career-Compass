using System.Net;
using System.Net.Mail;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace CareerCompass.Infrastructure.Services;

internal class SmtpEmailService : IEmailSender, IDisposable
{
    private readonly SmtpClient _client;

    public SmtpEmailService(SmtpSettings settings)
    {
        _client = new SmtpClient(settings.Host, settings.Port);
        _client.EnableSsl = settings.EnableSsl;
        _client.Credentials = new NetworkCredential(settings.UserName, settings.Password);
    }

    public void Dispose()
    {
        _client.Dispose();
    }


    public async Task Send(IMail mail, CancellationToken? cancellationToken = null)
    {
        var mailMessage = mail.ToMailMessage();
        await _client.SendMailAsync(mailMessage, cancellationToken ?? CancellationToken.None);
    }
}
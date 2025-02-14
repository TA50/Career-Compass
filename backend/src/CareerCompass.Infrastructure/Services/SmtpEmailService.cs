using System.Net;
using System.Net.Mail;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace CareerCompass.Infrastructure.Services;

internal class SmtpEmailService : IEmailSender, IDisposable
{
    private readonly SmtpClient _client;

    public SmtpEmailService(IOptions<SmtpSettings> options)
    {
        _client = new SmtpClient(options.Value.Host, options.Value.Port);
        _client.EnableSsl = options.Value.EnableSsl;
        _client.Credentials = new NetworkCredential(options.Value.UserName, options.Value.Password);
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
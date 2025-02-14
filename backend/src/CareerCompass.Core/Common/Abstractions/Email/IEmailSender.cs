namespace CareerCompass.Core.Common.Abstractions.Email;

public interface IEmailSender
{
    Task Send(IMail mail, CancellationToken? cancellationToken = null);
}
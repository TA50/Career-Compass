namespace CareerCompass.Core.Common.Abstractions;

public interface IEmailSender
{
    Task Send(string email, string subject, string message);
}
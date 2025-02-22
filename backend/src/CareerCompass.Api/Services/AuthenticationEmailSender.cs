using CareerCompass.Api.Emails;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Email;
using ErrorOr;

namespace CareerCompass.Api.Services;

public class AuthenticationEmailSender(
    IEmailSender emailSender,
    IConfiguration conf,
    CoreSettings coreSettings,
    ILoggerAdapter<AuthenticationEmailSender> logger)
{
    public async Task<ErrorOr<bool>> SendConfirmationEmail(string email, string code,
        CancellationToken cancellationToken)
    {
        var from = conf["RegistrationSender"];
        if (string.IsNullOrEmpty(from))
        {
            logger.LogError("RegistrationSender is not configured");
            return Error.Failure(ApiError.RegistrationSenderConfigNotFound, "Internal error");
        }


        var mail = new EmailConfirmationEmail(from, email, code, coreSettings.EmailConfirmationCodeLifetimeInHours);

        try
        {
            await emailSender.Send(mail, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send email to user with email {Email}", email);
            return Error.Failure(ApiError.FailedToSendEmailConfirmationEmail, "Internal error");
        }

        return true;
    }

    public async Task<ErrorOr<bool>> SendForgotPasswordEmail(
        string email, string code, CancellationToken cancellationToken)
    {
        var from = conf["RegistrationSender"];
        if (string.IsNullOrEmpty(from))
        {
            logger.LogError("RegistrationSender is not configured");
            return Error.Failure(ApiError.RegistrationSenderConfigNotFound, "Internal error");
        }


        var mail = new ForgotPasswordEmail(from, email, code, coreSettings.ForgotPasswordCodeLifetimeInHours);

        try
        {
            await emailSender.Send(mail, cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send email to user with email {Email}", email);
            return Error.Failure(ApiError.FailedToSendEmailConfirmationEmail, "Internal error");
        }
    }
}
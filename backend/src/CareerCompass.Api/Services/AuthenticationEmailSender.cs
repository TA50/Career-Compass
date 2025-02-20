using System.Web;
using CareerCompass.Api.Controllers;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Core.Users;
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


        var mail = new PlainTextMail(from, email).WithSubject("Welcome to Career Compass")
            .WithBody($@"
            Welcome to Career Compass!.

            Please confirm your email using the code below.
            {code}.

            This code will expire in {coreSettings.EmailConfirmationCodeLifetimeInHours} hours.


            If you did not register, please ignore this email.
            ");

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


        var mail = new PlainTextMail(from, email).WithSubject(@"Change your password")
            .WithBody($@"
            You have requested to change your password. Use the code below to change your password. 
            {code}.

            This code will expire in {coreSettings.ForgotPasswordCodeLifetimeInHours} hours.

            If you did not request to change your password, please ignore this email.
            ");

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
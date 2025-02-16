using CareerCompass.Api.Controllers;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Email;
using CareerCompass.Core.Users;
using ErrorOr;

namespace CareerCompass.Api.Services;

public class AuthenticationEmailSender(
    IEmailSender emailSender,
    IConfiguration conf,
    LinkGenerator linkGenerator,
    ILoggerAdapter<AuthenticationEmailSender> logger)
{
    public async Task<ErrorOr<bool>> SendConfirmationEmail(HttpContext context, UserId userId, string email,
        string code,
        string returnUrl,
        CancellationToken cancellationToken)
    {
        var from = conf["RegistrationSender"];
        if (string.IsNullOrEmpty(from))
        {
            logger.LogError("RegistrationSender is not configured");
            return Error.Failure(ApiError.RegistrationSenderConfigNotFound, "Internal error");
        }


        var confirmUrl = linkGenerator.GetUriByName(context,
            nameof(UserController.ConfirmEmail), values: new { userId = userId, code = code, returnUrl });


        if (string.IsNullOrEmpty(confirmUrl))
        {
            logger.LogError("Failed to generate confirmation URL for user with id {UserId}", userId);
            var error = Error.Failure(ApiError.FailedToGenerateEmailConfirmationLink, "Internal error");
            return error;
        }


        var mail = new PlainTextMail(from, email).WithSubject(@"Welcome to Career Compass")
            .WithBody($@"
            Welcome to Career Compass! Please click the link below to verify your email address. 
            {confirmUrl}
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
}
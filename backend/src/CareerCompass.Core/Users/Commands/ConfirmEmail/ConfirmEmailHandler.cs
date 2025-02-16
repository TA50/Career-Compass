using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.ConfirmEmail;

public class ConfirmEmailHandler(
    IUserRepository userRepository,
    ILoggerAdapter<ConfirmEmailHandler> logger)
    : IRequestHandler<ConfirmEmailCommand, ErrorOr<ConfirmEmailCommandResult>>
{
    public async Task<ErrorOr<ConfirmEmailCommandResult>> Handle(ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Confirming email for user with id {UserId}", request.UserId);
        var user = await userRepository.Get(request.UserId, true, cancellationToken);
        if (user is null)
        {
            return UserErrors.UserEmailConfirmation_UserNotFound(request.UserId);
        }

        var confirmationResult = user.ConfirmEmail(request.Code);
        if (!confirmationResult)
        {
            return UserErrors.UserEmailConfirmation_InvalidEmailConfirmationCode(request.UserId);
        }

        var result = await userRepository.Save(cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to confirm email for user with id {UserId}: {ErrorMessage}", request.UserId,
                result.ErrorMessage ?? "Unknown error");

            return UserErrors.UserEmailConfirmation_ConfirmationFailed(request.UserId);
        }

        logger.LogInformation("Email confirmed for user with id {UserId}", request.UserId);
        return new ConfirmEmailCommandResult(request.UserId);
    }
}
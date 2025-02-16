using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.ChangeEmail;

public class ChangeEmailCommandHandler(
    IUserRepository userRepository,
    ICryptoService cryptoService,
    ILoggerAdapter<ChangeEmailCommandHandler> logger)
    : IRequestHandler<ChangeEmailCommand, ErrorOr<ChangeEmailCommandResult>>
{
    public async Task<ErrorOr<ChangeEmailCommandResult>> Handle(ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating email for user with id {UserId}", request.UserId);

        var user = await userRepository.Get(request.UserId, true, cancellationToken);
        if (user is null)
        {
            return UserErrors.ChangeEmail_UserNotFound(request.UserId);
        }

        var spec = new GetUserByEmailSpecification(request.NewEmail)
            .ExcludeUser(user.Id);
        var emailExists =
            await userRepository.Exists(spec, cancellationToken);

        if (emailExists)
        {
            return UserErrors.ChangeEmail_EmailAlreadyExists(request.NewEmail);
        }

        var passwordsMatch = cryptoService.Verify(request.OldPassword, user.Password);
        if (!passwordsMatch)
        {
            return UserErrors.ChangeEmail_InvalidCredentials(request.UserId);
        }

        user.ChangeEmail(request.NewEmail);
        var confirmationCode = user.GenerateEmailConfirmationCode();
        var result = await userRepository.Save(cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to update email for user with id {UserId}: {ErrorMessage}", user.Id,
                result.ErrorMessage ?? "Unknown error");

            return UserErrors.ChangeEmail_OperationFailed(request.UserId);
        }

        logger.LogInformation("Email updated for user with id {UserId}", user.Id);


        return new ChangeEmailCommandResult(user.Id, confirmationCode);
    }
}
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    ICryptoService cryptoService,
    ILoggerAdapter<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, ErrorOr<ResetPasswordCommandResult>>
{
    public async Task<ErrorOr<ResetPasswordCommandResult>> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Resetting password for user with id {UserId}", request.UserId);
        var user = await userRepository.Get(request.UserId, true, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("Failed to reset password for user with id {UserId}: user does not exist",
                request.UserId);
            return UserErrors.ResetPassword_InvalidCredentials(request.UserId);
        }

        var passwordsMatch = cryptoService.Verify(request.OldPassword, user.Password);
        if (!passwordsMatch)
        {
            logger.LogInformation("Failed to reset password for user with id {UserId}: old password does not match",
                request.UserId);
            return UserErrors.ResetPassword_InvalidCredentials(request.UserId);
        }

        user.SetPassword(request.NewPassword);
        var result = await userRepository.Save(cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to reset password for user with id {UserId}: {ErrorMessage}", request.UserId,
                result.ErrorMessage ?? "Unknown error");
            return UserErrors.ResetPassword_OperationFailed(request.UserId);
        }

        logger.LogInformation("Password reset for user with id {UserId}", request.UserId);

        return new ResetPasswordCommandResult(user.Id);
    }
}
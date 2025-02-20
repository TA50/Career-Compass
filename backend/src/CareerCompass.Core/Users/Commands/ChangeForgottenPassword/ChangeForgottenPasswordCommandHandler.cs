using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.ChangeForgottenPassword;

public class ChangeForgottenPasswordCommandHandler(
    IUserRepository userRepository,
    ICryptoService cryptoService,
    ILoggerAdapter<ChangeForgottenPasswordCommandHandler> logger) : IRequestHandler<
    ChangeForgottenPasswordCommand, ErrorOr<ChangeForgottenPasswordCommandResult>>
{
    public async Task<ErrorOr<ChangeForgottenPasswordCommandResult>> Handle(ChangeForgottenPasswordCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Changing forgotten password for {Email}", request.Email);
        var spec = new GetUserByEmailSpecification(request.Email).RequireConfirmation();
        var user = await userRepository.Single(spec, true, cancellationToken);
        if (user is null)
        {
            return UserErrors.ChangeForgotPassword_InvalidEmail(request.Email);
        }

        var confirmationResult = user.ConfirmForgotPassword(request.Code);
        if (!confirmationResult)
        {
            return UserErrors.ChangeForgotPassword_InvalidCode(request.Email);
        }

        var passHash = cryptoService.Hash(request.NewPassword);
        user.SetPassword(passHash);
        var result = await userRepository.Save(cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to change forgotten password for {Email}. Error: {Error}", request.Email,
                result.ErrorMessage ?? "Unknown error");
            return UserErrors.ChangeForgotPassword_OperationFailed(request.Email);
        }

        logger.LogInformation("Successfully changed forgotten password for {Email}", request.Email);

        return new ChangeForgottenPasswordCommandResult(user.Id, user.Email);
    }
}
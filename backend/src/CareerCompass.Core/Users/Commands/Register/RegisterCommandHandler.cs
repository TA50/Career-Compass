using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    ICryptoService cryptoService,
    CoreSettings settings,
    ILoggerAdapter<RegisterCommandHandler> logger
)
    : IRequestHandler<RegisterCommand, ErrorOr<RegisterCommandResult>>
{
    public async Task<ErrorOr<RegisterCommandResult>> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating user with email {Email}", request.Email);

        var spec = new GetUserByEmailSpecification(request.Email);
        var userWithEmailExists = await userRepository.Exists(spec, cancellationToken);
        if (userWithEmailExists)
        {
            return UserErrors.UserCreation_UserWithEmailExists(request.Email);
        }

        var user = User.Create(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            password: cryptoService.Hash(request.Password));

        var confirmationCode =
            user.GenerateEmailConfirmationCode(TimeSpan.FromHours(settings.EmailConfirmationCodeLifetimeInHours));
        await userRepository.StartTransaction();
        var result = await userRepository.Create(user, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create user with email {Email}: {ErrorMessage}", request.Email,
                result.ErrorMessage ?? "Unknown error");

            await userRepository.RollbackTransaction();
            return UserErrors.UserCreation_CreationFailed(request.Email);
        }

        await userRepository.CommitTransaction();
        logger.LogInformation("User with email {Email} was successfully created", request.Email);
        return new RegisterCommandResult(user.Id, user.Email, confirmationCode);
    }
}
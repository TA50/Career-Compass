using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    ICryptoService cryptoService,
    ILoggerAdapter<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, ErrorOr<LoginCommandResult>>
{
    public async Task<ErrorOr<LoginCommandResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying to login user with email: {Email}", request.Email);
        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();
        var user = await userRepository.Single(spec, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("user not found with this email: {Email}", request.Email);
            return UserErrors.UserLogin_InvalidCredentials(request.Email);
        }

        var passwordIsValid = cryptoService.Verify(request.Password,
            user.Password);

        if (!passwordIsValid)
        {
            logger.LogInformation("provided wrong password for user with email: {Email}", request.Email);
            return UserErrors.UserLogin_InvalidCredentials(request.Email);
        }


        logger.LogInformation("Login successful for user {Email}", request.Email);
        return new LoginCommandResult(user.Id);
    }
}
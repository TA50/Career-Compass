using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using ErrorOr;
using MediatR;


namespace CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;

public class GenerateForgotPasswordCodeCommandHandler(
    IUserRepository userRepository,
    ILoggerAdapter<GenerateForgotPasswordCodeCommandHandler> logger)
    : IRequestHandler<GenerateForgotPasswordCodeCommand,
        ErrorOr<GenerateForgotPasswordCodeCommandResult>>
{
    public async Task<ErrorOr<GenerateForgotPasswordCodeCommandResult>> Handle(
        GenerateForgotPasswordCodeCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Forgot password code generated for {Email}", request.Email);
        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();

        var user = await userRepository.Single(spec, cancellationToken);

        if (user is null)
        {
            return UserErrors.ForgotPassword_InvalidEmail(request.Email);
        }

        var code = user.GenerateForgotPasswordCode();
        var result = await userRepository.Save(cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("Forgot password code generation failed for {Email}. Error: {Error}",
                request.Email,
                result.ErrorMessage ?? "Unknown error");

            return UserErrors.ForgotPassword_OperationFailed(request.Email);
        }

        logger.LogInformation("Forgot password code generated successfully for {Email}", request.Email);
        return new GenerateForgotPasswordCodeCommandResult(Email: user.Email, Code: code);
    }
}
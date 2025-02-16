using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using ErrorOr;
using MediatR;


namespace CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;

public class GenerateForgotPasswordCodeCommandHandler(
    IUserRepository userRepository,
    ILoggerAdapter<GenerateForgotPasswordCodeCommandHandler> logger)
    : IRequestHandler<GenerateForgotPasswordCodeCommand,
        ErrorOr<GenerateForgotPasswordCodeCommandResult>>
{
    public Task<ErrorOr<GenerateForgotPasswordCodeCommandResult>> Handle(GenerateForgotPasswordCodeCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
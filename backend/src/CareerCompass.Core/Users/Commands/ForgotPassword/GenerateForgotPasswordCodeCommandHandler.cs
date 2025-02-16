using ErrorOr;
using MediatR;


namespace CareerCompass.Core.Users.Commands.ForgotPassword;

public class GenerateForgotPasswordCodeCommandHandler : IRequestHandler<GenerateForgotPasswordCodeCommand,
    ErrorOr<GenerateForgotPasswordCodeCommandResult>>
{
    public Task<ErrorOr<GenerateForgotPasswordCodeCommandResult>> Handle(GenerateForgotPasswordCodeCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
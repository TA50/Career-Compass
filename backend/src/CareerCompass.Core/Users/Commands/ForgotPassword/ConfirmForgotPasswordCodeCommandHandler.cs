using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.ForgotPassword;

public class ConfirmForgotPasswordCodeCommandHandler : IRequestHandler<ConfirmForgotPasswordCodeCommand, ErrorOr<ConfirmForgotPasswordCodeCommandResult>>
{
    public Task<ErrorOr<ConfirmForgotPasswordCodeCommandResult>> Handle(ConfirmForgotPasswordCodeCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.ForgotPassword;

public record ConfirmForgotPasswordCodeCommandResult(
    UserId UserId
);

public record ConfirmForgotPasswordCodeCommand(
    string Email,
    string Code
) : IRequest<ErrorOr<ConfirmForgotPasswordCodeCommandResult>>;
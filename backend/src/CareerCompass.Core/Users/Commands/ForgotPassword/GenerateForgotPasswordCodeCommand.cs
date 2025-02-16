using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.ForgotPassword;

public record GenerateForgotPasswordCodeCommandResult(
    string Code,
    string Email
);

public record GenerateForgotPasswordCodeCommand(
    string Email
) : IRequest<ErrorOr<GenerateForgotPasswordCodeCommandResult>>;
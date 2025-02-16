using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;

public record GenerateForgotPasswordCodeCommandResult(
    string Code,
    string Email
);

public record GenerateForgotPasswordCodeCommand(
    string Email
) : IRequest<ErrorOr<GenerateForgotPasswordCodeCommandResult>>;
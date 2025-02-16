using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.ChangeForgottenPassword;

public record ChangeForgottenPasswordCommandResult(
    UserId UserId,
    string Email
);

public record ChangeForgottenPasswordCommand(
    string Email,
    string Code,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<ErrorOr<ChangeForgottenPasswordCommandResult>>;
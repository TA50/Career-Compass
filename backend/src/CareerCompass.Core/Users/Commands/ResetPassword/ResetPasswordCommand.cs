using MediatR;
using ErrorOr;


namespace CareerCompass.Core.Users.Commands.ResetPassword;

public record ResetPasswordCommandResult(
    UserId UserId
);

public record ResetPasswordCommand(
    UserId UserId,
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<ErrorOr<ResetPasswordCommandResult>>;
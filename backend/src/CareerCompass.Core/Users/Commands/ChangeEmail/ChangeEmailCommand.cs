using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.ChangeEmail;

public record ChangeEmailCommandResult(
    UserId UserId,
    string EmailConfirmationCode
);

public record ChangeEmailCommand(
    UserId UserId,
    string OldPassword,
    string NewEmail
) : IRequest<ErrorOr<ChangeEmailCommandResult>>;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.ConfirmEmail;

public record ConfirmEmailCommandResult(UserId UserId);

public record ConfirmEmailCommand(UserId UserId, string Code) : IRequest<ErrorOr<ConfirmEmailCommandResult>>;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.Login;

public record LoginCommandResult(UserId UserId);

public record LoginCommand(string Email, string Password) : IRequest<ErrorOr<LoginCommandResult>>;
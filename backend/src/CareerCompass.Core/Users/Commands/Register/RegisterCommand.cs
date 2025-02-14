using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.Register;

public record RegisterCommandResult(
    UserId UserId,
    string Email,
    string ConfirmationCode);

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<ErrorOr<RegisterCommandResult>>;
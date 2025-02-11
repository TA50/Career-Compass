using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.Register;

public record RegisterCommandResult();

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<ErrorOr<RegisterCommandResult>>;
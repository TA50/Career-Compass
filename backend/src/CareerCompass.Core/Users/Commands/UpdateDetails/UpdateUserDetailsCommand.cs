using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.UpdateDetails;

public record UpdateUserDetailsCommand(
    UserId UserId,
    string FirstName,
    string LastName
) : IRequest<ErrorOr<User>>;
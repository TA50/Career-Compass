using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Users.Commands.UpdateDetails;

public record UpdateUserDetailsCommand(
    string UserId,
    string FirstName,
    string LastName
) : IRequest<ErrorOr<User>>;
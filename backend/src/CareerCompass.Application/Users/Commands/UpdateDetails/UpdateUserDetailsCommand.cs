using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Users.Commands.UpdateDetails;

public record UpdateUserDetailsCommand(
    string FirstName,
    string LastName
) : IRequest<User>, IRequest<ErrorOr<User>>;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Queries.GetUserById;

public record GetUserByIdQuery(
    UserId UserId
) : IRequest<ErrorOr<User>>;
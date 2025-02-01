using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Users.Queries.GetUser;

public record GetUserByIdentityIdQuery(
    string IdentityId
) : IRequest<ErrorOr<User>>;
using CareerCompass.Application.Tags;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Users.Queries.GetUser;

public class GetUserByIdentityIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdentityIdQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByIdentityIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetFromIdentity(request.IdentityId, cancellationToken);

        if (user == null)
        {
            return UserErrors.UserQuery_UserNotFound( new UserId(request.IdentityId));
        }

        return user;
    }
}
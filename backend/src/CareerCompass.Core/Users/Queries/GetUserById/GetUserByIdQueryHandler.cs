using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.Get(request.UserId, cancellationToken);
        if (user is null)
        {
            return UserErrors.UserQuery_UserNotFound(request.UserId);
        }

        return user;
    }
}
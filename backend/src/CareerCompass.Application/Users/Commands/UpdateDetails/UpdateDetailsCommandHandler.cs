using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Users.Commands.UpdateDetails;

public class UpdateUserDetailsCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserDetailsCommand, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        List<Error> errors = [];
        var user = await userRepository.Get(request.UserId, cancellationToken);

        if (user == null)
        {
            errors.Add(UserErrors.UserModification_UserNotFound(request.UserId));
        }
        else
        {
            user.SetName(request.FirstName, request.LastName);
            var result = await userRepository.Update(user, cancellationToken);
            return result;
        }

        return ErrorOr<User>.From(errors);
    }
}
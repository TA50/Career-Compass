using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.UpdateDetails;

public class UpdateUserDetailsCommandHandler(
    IUserRepository userRepository,
    ILoggerAdapter<UpdateUserDetailsCommandHandler> logger)
    : IRequestHandler<UpdateUserDetailsCommand, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user details for user {@UserId}", request.UserId);
        List<Error> errors = [];
        var user = await userRepository.Get(request.UserId,
            true,
            cancellationToken);

        if (user is null)
        {
            errors.Add(UserErrors.UserModification_UserNotFound(request.UserId));
            return ErrorOr<User>.From(errors);
        }

        user.SetName(request.FirstName, request.LastName);
        var result = await userRepository.Save();
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to update user details for user {@UserId}", request.UserId);
            errors.Add(UserErrors.UserModification_ModificationFailed(request.UserId));
            return ErrorOr<User>.From(errors);
        }

        logger.LogInformation("Updated user details for user {@UserId}", request.UserId);


        return user;
    }
}
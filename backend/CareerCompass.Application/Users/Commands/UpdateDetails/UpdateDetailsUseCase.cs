using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Users.Commands.UpdateDetails;

public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, ErrorOr<User>>
{
    public Task<ErrorOr<User>> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
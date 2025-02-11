using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<RegisterCommandResult>>
{
    public Task<ErrorOr<RegisterCommandResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
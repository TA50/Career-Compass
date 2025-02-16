using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Users.Commands.ChangeForgottenPassword;

public class ChangeForgottenPasswordCommandHandler(
    IUserRepository userRepository,
    ICryptoService cryptoService,
    ILoggerAdapter<ChangeForgottenPasswordCommandHandler> logger) : IRequestHandler<
    ChangeForgottenPasswordCommand, ErrorOr<ChangeForgottenPasswordCommandResult>>
{
    public async Task<ErrorOr<ChangeForgottenPasswordCommandResult>> Handle(ChangeForgottenPasswordCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
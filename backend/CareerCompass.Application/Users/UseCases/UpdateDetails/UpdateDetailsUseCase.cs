using MediatR;

namespace CareerCompass.Application.Users.UseCases.UpdateDetails;

public class UpdateUserDetailsUseCase : IRequestHandler<UpdateUserDetailsInput, UpdateUserDetailsOutput>
{
    public Task<UpdateUserDetailsOutput> Handle(UpdateUserDetailsInput request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
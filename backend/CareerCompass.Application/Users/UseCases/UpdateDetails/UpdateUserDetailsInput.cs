using MediatR;

namespace CareerCompass.Application.Users.UseCases.UpdateDetails;

public record UpdateUserDetailsInput(
    string firstName,
    string lastName
) : IRequest<User>, IRequest<UpdateUserDetailsOutput>;
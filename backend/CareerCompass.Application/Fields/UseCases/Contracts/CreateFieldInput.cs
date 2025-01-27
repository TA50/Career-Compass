using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Fields.UseCases.Contracts;

public record CreateFieldInput(UserId UserId, string Name) : IRequest<ErrorOr<Field>>;
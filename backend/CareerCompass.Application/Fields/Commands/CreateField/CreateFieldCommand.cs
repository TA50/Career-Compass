using CareerCompass.Application.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Fields.Commands.CreateField;

public record CreateFieldCommand(UserId UserId, string Name) : IRequest<ErrorOr<Field>>;
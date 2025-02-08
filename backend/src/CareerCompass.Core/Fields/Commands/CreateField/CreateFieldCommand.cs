using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Fields.Commands.CreateField;

public record CreateFieldCommand(
    UserId UserId,
    string Name,
    string Group)
    : IRequest<ErrorOr<Field>>;
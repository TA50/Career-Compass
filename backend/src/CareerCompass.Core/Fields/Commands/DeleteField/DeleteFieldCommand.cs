using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Fields.Commands.DeleteField;

public record DeleteFieldCommand(UserId UserId, FieldId Id) : IRequest<ErrorOr<Unit>>;
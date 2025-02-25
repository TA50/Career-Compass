using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Tags.Commands.Delete;

public record DeleteTagCommand(UserId UserId, TagId Id) : IRequest<ErrorOr<Unit>>;
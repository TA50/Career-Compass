using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Tags.Commands.Create;

public record CreateTagCommand(UserId UserId, string Name) : IRequest<ErrorOr<Tag>>;
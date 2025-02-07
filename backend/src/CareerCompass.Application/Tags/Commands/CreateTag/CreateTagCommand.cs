using CareerCompass.Application.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Tags.Commands.CreateTag;

public record CreateTagCommand(UserId UserId, string Name) : IRequest<ErrorOr<Tag>>;
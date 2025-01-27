using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Tags.UseCases.Contracts;

public record CreateTagInput(UserId UserId, string Name) : IRequest<ErrorOr<Tag>>;
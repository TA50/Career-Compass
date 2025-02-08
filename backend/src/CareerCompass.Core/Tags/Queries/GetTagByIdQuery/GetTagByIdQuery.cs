using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;


namespace CareerCompass.Core.Tags.Queries.GetTagByIdQuery;

public record GetTagByIdQuery(UserId UserId, TagId TagId) : IRequest<ErrorOr<Tag>>;
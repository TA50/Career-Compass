using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;


namespace CareerCompass.Core.Tags.Queries.GetTagsQuery;

public record GetTagsQuery(UserId UserId) : IRequest<ErrorOr<IList<Tag>>>;
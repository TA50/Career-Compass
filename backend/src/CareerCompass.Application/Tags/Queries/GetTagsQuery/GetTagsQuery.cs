using MediatR;
using ErrorOr;


namespace CareerCompass.Application.Tags.Queries.GetTagsQuery;

public record GetTagsQuery(string UserId) : IRequest<ErrorOr<IList<Tag>>>;
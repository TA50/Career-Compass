using MediatR;
using ErrorOr;


namespace CareerCompass.Application.Tags.Queries.GetTagByIdQuery;

public record GetTagByIdQuery(string UserId, string TagId) : IRequest<ErrorOr<Tag>>;
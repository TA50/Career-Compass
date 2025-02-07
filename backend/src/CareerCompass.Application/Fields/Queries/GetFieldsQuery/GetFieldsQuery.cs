using MediatR;
using ErrorOr;


namespace CareerCompass.Application.Fields.Queries.GetFieldsQuery;

public record GetFieldsQuery(string UserId) : IRequest<ErrorOr<IList<Field>>>;
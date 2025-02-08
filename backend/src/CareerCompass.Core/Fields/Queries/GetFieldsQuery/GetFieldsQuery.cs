using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;


namespace CareerCompass.Core.Fields.Queries.GetFieldsQuery;

public record GetFieldsQuery(UserId UserId) : IRequest<ErrorOr<IList<Field>>>;
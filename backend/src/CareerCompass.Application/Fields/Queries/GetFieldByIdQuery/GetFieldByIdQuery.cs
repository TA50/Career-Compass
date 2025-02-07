using MediatR;
using ErrorOr;


namespace CareerCompass.Application.Fields.Queries.GetFieldByIdQuery;

public record GetFieldByIdQuery(string UserId, string FieldId) : IRequest<ErrorOr<Field>>;
using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;


namespace CareerCompass.Core.Fields.Queries.GetFieldByIdQuery;

public record GetFieldByIdQuery(UserId UserId, FieldId FieldId) : IRequest<ErrorOr<Field>>;
using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Fields.Queries.GetFieldByIdQuery;

public class GetFieldByIdQueryHandler(IFieldRepository fieldRepository)
    : IRequestHandler<GetFieldByIdQuery, ErrorOr<Field>>
{
    public async Task<ErrorOr<Field>> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await fieldRepository
            .Get(new UserId(request.UserId),
                new FieldId(request.FieldId),
                cancellationToken);

        return tag ?? ErrorOr<Field>.From([FieldErrors.FieldRead_FieldNotFound(request.UserId, request.FieldId)]);
    }
}
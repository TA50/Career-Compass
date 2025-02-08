using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Fields.Queries.GetFieldByIdQuery;

public class GetFieldByIdQueryHandler(IFieldRepository fieldRepository, ILoggerAdapter<GetFieldByIdQueryHandler> logger)
    : IRequestHandler<GetFieldByIdQuery, ErrorOr<Field>>
{
    public async Task<ErrorOr<Field>> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting field for user {@UserId} {@FieldId}", request.UserId, request.FieldId);
        var spec = new GetFieldByIdSpecification(request.FieldId, request.UserId);

        var field = await fieldRepository.Single(spec, cancellationToken);

        return field ?? ErrorOr<Field>.From([
            FieldErrors.FieldRead_FieldNotFound(request.UserId, request.FieldId)
        ]);
    }
}
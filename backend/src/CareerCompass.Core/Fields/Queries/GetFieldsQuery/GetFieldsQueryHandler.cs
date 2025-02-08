using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Fields;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Fields.Queries.GetFieldsQuery;

public class GetFieldsQueryHandler(IFieldRepository fieldRepository, ILoggerAdapter<GetFieldsQueryHandler> logger)
    : IRequestHandler<GetFieldsQuery, ErrorOr<IList<Field>>>
{
    public async Task<ErrorOr<IList<Field>>> Handle(GetFieldsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting fields for user {@UserId}", request.UserId);
        var spec = new GetUserFieldsSpecification(request.UserId);
        var fields = await fieldRepository.Get(spec,
            cancellationToken);
        return ErrorOrFactory.From(fields);
    }
}
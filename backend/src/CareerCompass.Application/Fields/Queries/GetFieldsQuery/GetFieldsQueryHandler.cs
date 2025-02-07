using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Fields.Queries.GetFieldsQuery;

public class GetFieldsQueryHandler(IFieldRepository fieldRepository)
    : IRequestHandler<GetFieldsQuery, ErrorOr<IList<Field>>>
{
    public async Task<ErrorOr<IList<Field>>> Handle(GetFieldsQuery request, CancellationToken cancellationToken)

    {
        // We don't need to check if user exists, if the user does not exist, then no fields will be returned !

        var fields = await fieldRepository.Get(new UserId(request.UserId), cancellationToken);
        return ErrorOrFactory.From(fields);
    }
}
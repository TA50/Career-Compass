using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Tags.Queries.GetTagsQuery;

public class GetTagsQueryHandler(ITagRepository tagRepository) : IRequestHandler<GetTagsQuery, ErrorOr<IList<Tag>>>
{
    public async Task<ErrorOr<IList<Tag>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)

    {
        // We don't need to check if user exists, if the user does not exist, then no tags will be returned !

        var tags = await tagRepository.Get(new UserId(request.UserId), cancellationToken);
        return ErrorOrFactory.From(tags);
    }
}
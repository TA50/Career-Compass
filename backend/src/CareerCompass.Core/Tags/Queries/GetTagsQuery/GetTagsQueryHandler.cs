using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Tags.Queries.GetTagsQuery;

public class GetTagsQueryHandler(ITagRepository tagRepository, ILoggerAdapter<GetTagsQueryHandler> logger)
    : IRequestHandler<GetTagsQuery, ErrorOr<IList<Tag>>>
{
    public async Task<ErrorOr<IList<Tag>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)

    {
        logger.LogInformation("Getting tags for user {UserId}", request.UserId);
        var spec = new GetUserTagsSpecification(request.UserId);
        var tags = await tagRepository.Get(spec, cancellationToken);
        logger.LogInformation("Found {TagsCount} tags for user {@UserId}", tags.Count, request.UserId);
        return ErrorOrFactory.From(tags);
    }
}
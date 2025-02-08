using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Tags.Queries.GetTagByIdQuery;

public class GetTagByIdQueryHandler(ITagRepository tagRepository, ILoggerAdapter<GetTagByIdQueryHandler> logger)
    : IRequestHandler<GetTagByIdQuery, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting tag for user {@UserId} {@TagId}", request.UserId, request.TagId);
        var spec = new GetTagByIdSpecification(request.TagId, request.UserId);
        var tag = await tagRepository.Single(spec, cancellationToken);

        if (tag is null)
        {
            return ErrorOr<Tag>.From([
                TagErrors.TagRead_TagNotFound(request.UserId, request.TagId)
            ]);
        }

        logger.LogInformation("Found tag for user {@UserId} {@TagId}", request.UserId, request.TagId);
        return tag;
    }
}
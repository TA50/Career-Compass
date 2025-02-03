using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;

namespace CareerCompass.Application.Tags.Queries.GetTagByIdQuery;

public class GetTagByIdQueryHandler(ITagRepository tagRepository)
    : IRequestHandler<GetTagByIdQuery, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.Get(new UserId(request.UserId),
            new TagId(request.TagId),
            cancellationToken);

        return tag ?? ErrorOr<Tag>.From([TagErrors.TagRead_TagNotFound(request.UserId, request.TagId)]);
    }
}
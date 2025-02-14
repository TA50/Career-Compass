using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Common.Specifications.Users;
using CareerCompass.Core.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Tags.Commands.CreateTag;

public class CreateTagCommandHandler(
    ITagRepository tagRepository,
    IUserRepository userRepository,
    ILoggerAdapter<CreateTagCommandHandler> logger
) : IRequestHandler<CreateTagCommand, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating tag for user {@UserId} {@TagName}", request.UserId, request.Name);
        var errors = new List<Error>();
        // Validate User Id

        var userExists = await userRepository.Exists(request.UserId, cancellationToken);

        if (!userExists)
        {
            errors.Add(TagErrors.TagCreation_UserNotFound(request.UserId));
        }

        // Validate Tag Name
        var spec = new GetTagByNameSpecification(request.UserId, request.Name);
        var tagExists = await tagRepository.Exists(spec, cancellationToken);

        if (tagExists)
        {
            errors.Add(TagErrors.TagCreation_TagNameAlreadyExists(request.UserId, request.Name));
        }

        if (errors.Any())
        {
            return ErrorOr<Tag>.From(errors);
        }

        var tag = Tag.Create(request.UserId, request.Name);

        var result = await tagRepository.Create(tag, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create tag for user {@UserId} {@TagName}. Failed with message: {@Message}",
                request.UserId, request.Name, result.ErrorMessage ?? "Unknown error");
            return ErrorOr<Tag>.From([
                TagErrors.TagCreation_FailedToCreateTag(
                    request.UserId, request.Name
                )
            ]);
        }

        logger.LogInformation("Tag created for user {@UserId} {@TagName}", request.UserId, request.Name);

        return tag;
    }
}
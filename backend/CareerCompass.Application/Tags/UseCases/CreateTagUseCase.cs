using CareerCompass.Application.Tags.UseCases.Contracts;
using CareerCompass.Application.Users;
using MediatR;
using ErrorOr;


namespace CareerCompass.Application.Tags.UseCases;

public class CreateTagUseCase(
    ITagRepository tagRepository,
    IUserRepository userRepository
) : IRequestHandler<CreateTagInput, ErrorOr<Tag>>
{
    public async Task<ErrorOr<Tag>> Handle(CreateTagInput request, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();
        // Validate User Id
        var userExists = await userRepository.Exists(request.UserId, cancellationToken);

        if (!userExists)
        {
            errors.Add(TagErrors.TagValidation_UserNotFound(request.UserId));
        }

        // Validate Tag Name
        var tagExists = await tagRepository.Exists(request.UserId, request.Name, cancellationToken);

        if (tagExists)
        {
            errors.Add(TagErrors.TagValidation_TagNameAlreadyExists(request.UserId, request.Name));
        }

        if (errors.Any())
        {
            return ErrorOr<Tag>.From(errors);
        }

        var tag = new Tag(TagId.NewId(), request.Name, request.UserId);

        return await tagRepository.Create(tag, cancellationToken);
    }
}
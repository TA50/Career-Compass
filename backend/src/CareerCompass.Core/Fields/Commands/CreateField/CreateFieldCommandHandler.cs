using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Common.Specifications.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Core.Fields.Commands.CreateField;

public class CreateFieldCommandHandler(
    IFieldRepository fieldRepository,
    IUserRepository userRepository,
    ILoggerAdapter<CreateFieldCommandHandler> logger)
    : IRequestHandler<CreateFieldCommand, ErrorOr<Field>>
{
    public async Task<ErrorOr<Field>> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating field {Name} inside group {Group} for user {UserId}", request.Name,
            request.Group, request.UserId);
        var errors = new List<Error>();
        // Validate User 

        var userExists = await userRepository.Exists(
            request.UserId, cancellationToken);
        if (!userExists)
        {
            errors.Add(FieldErrors.FieldValidation_UserNotFound(request.UserId));
        }

        // Validate Field Name
        var spec = new GetUserFieldByNameAndGroupSpecification(request.UserId, request.Name, request.Group);
        var fieldExists = await fieldRepository.Exists(spec, cancellationToken);

        if (fieldExists)
        {
            errors.Add(FieldErrors.FieldValidation_NameAlreadyExists(request.UserId, request.Name, request.Group));
        }

        if (errors.Any())
        {
            return ErrorOr<Field>.From(errors);
        }

        var field = Field.Create(
            name: request.Name,
            userId: request.UserId,
            group: request.Group
        );

        var result = await fieldRepository.Create(field, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError(
                "Failed to create field {Name} inside group {Group} for user {UserId}. Failed because: {Message}",
                request.Name,
                request.Group, request.UserId, result.ErrorMessage ?? "Unknown error");

            return ErrorOr<Field>.From([
                FieldErrors.FieldCreation_FailedToCreateField(
                    request.UserId, request.Name, request.Group
                )
            ]);
        }

        logger.LogInformation("Created field {Name} inside group {Group} for user {UserId}", request.Name,
            request.Group, request.UserId);

        return field;
    }
}
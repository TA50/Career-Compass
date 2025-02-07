using CareerCompass.Application.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Fields.Commands.CreateField;

public class CreateFieldCommandHandler(IFieldRepository fieldRepository, IUserRepository userRepository)
    : IRequestHandler<CreateFieldCommand, ErrorOr<Field>>
{
    public async Task<ErrorOr<Field>> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();
        // Validate User 
        var userExists = await userRepository.Exists(request.UserId, cancellationToken);
        if (!userExists)
        {
            errors.Add(FieldErrors.FieldValidation_UserNotFound(request.UserId));
        }

        // Validate Field Name

        var fieldExists = await fieldRepository.Exists(request.UserId, request.Name, cancellationToken);

        if (fieldExists)
        {
            errors.Add(FieldErrors.FieldValidation_NameAlreadyExists(request.UserId, request.Name));
        }

        if (errors.Any())
        {
            return ErrorOr<Field>.From(errors);
        }

        var field = new Field(id:
            FieldId.NewId(),
            name: request.Name,
            userId: request.UserId);

        return await fieldRepository.Create(field, cancellationToken);
    }
}
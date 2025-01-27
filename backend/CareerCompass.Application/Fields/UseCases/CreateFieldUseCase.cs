using CareerCompass.Application.Fields.UseCases.Contracts;
using CareerCompass.Application.Users;
using ErrorOr;
using MediatR;

namespace CareerCompass.Application.Fields.UseCases;

public class CreateFieldUseCase(IFieldRepository fieldRepository, IUserRepository userRepository)
    : IRequestHandler<CreateFieldInput, ErrorOr<Field>>
{
    private readonly IFieldRepository _fieldRepository = fieldRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<Field>> Handle(CreateFieldInput request, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();
        // Validate User 
        var userExists = await _userRepository.Exists(request.UserId, cancellationToken);
        if (!userExists)
        {
            errors.Add(FieldErrors.FieldValidation_UserNotFound(request.UserId));
        }

        // Validate Field Name

        var fieldExists = await _fieldRepository.Exists(request.UserId, request.Name, cancellationToken);

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

        return await _fieldRepository.Create(field, cancellationToken);
    }
}
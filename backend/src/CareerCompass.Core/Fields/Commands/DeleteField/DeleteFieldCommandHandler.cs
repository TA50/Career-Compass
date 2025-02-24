using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using MediatR;
using ErrorOr;

namespace CareerCompass.Core.Fields.Commands.DeleteField;

public class DeleteFieldCommandHandler(
    IFieldRepository fieldRepository,
    ILoggerAdapter<DeleteFieldCommandHandler> logger) : IRequestHandler<DeleteFieldCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        logger
            .LogInformation("Deleting field with id: {FieldId} for user: {UserId}", request.Id, request.UserId);

        var spec = new GetFieldByIdSpecification(request.Id, request.UserId);
        var exists = await fieldRepository.Exists(spec, cancellationToken);

        if (!exists)
        {
            return FieldErrors.FieldDelete_FieldNotFound(request.Id);
        }


        var result = await fieldRepository.Delete(request.Id, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError(
                "Failed to delete field with id: {FieldId} for user: {UserId}. Error: {ErrorMessage}",
                request.Id, request.UserId, result.ErrorMessage ?? "Unknown error");

            return FieldErrors.FieldDelete_OperationFailed(request.Id);
        }

        logger
            .LogInformation("Field with id: {FieldId} for user: {UserId} deleted", request.Id, request.UserId);

        return Unit.Value;
    }
}
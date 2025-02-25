using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Events;
using MediatR;
using ErrorOr;


namespace CareerCompass.Core.Tags.Commands.Delete;

public class DeleteTagCommandHandler(
    ITagRepository tagRepository,
    IPublisher publisher,
    ILoggerAdapter<DeleteTagCommandHandler> logger) : IRequestHandler<DeleteTagCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        logger
            .LogInformation("Deleting tag with id: {TagId} for user: {UserId}", request.Id, request.UserId);
        // logger.LogInformation("Deleting field with id: {TagId} for user: {UserId}", fieldId, userId);

        var spec = new GetTagByIdSpecification(request.Id, request.UserId);
        var exists = await tagRepository.Exists(spec, cancellationToken);
        if (!exists)
        {
            return TagErrors.TagDelete_TagNotFound(request.Id);
        }

        await tagRepository.StartTransaction(cancellationToken);
        var result = await tagRepository.Delete(request.Id, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError(
                "Failed to delete tag with id: {TagId} for user: {UserId}. Error: {ErrorMessage}",
                request.Id, request.UserId, result.ErrorMessage ?? "Unknown error");

            await tagRepository.RollbackTransaction(cancellationToken);

            return TagErrors.TagDelete_OperationFailed(request.Id);
        }

        logger
            .LogInformation("Tag with id: {TagId} for user: {UserId} deleted", request.Id, request.UserId);
        var evt = new TagDeletedEvent(request.Id);

        try
        {
            await publisher.Publish(evt, cancellationToken);
            logger.LogInformation("Published event: {EventName}", evt.GetType().Name);
            await tagRepository.CommitTransaction(cancellationToken);
            return Unit.Value;
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "Failed to publish event: {EventName}", nameof(TagDeletedEvent));

            await tagRepository.RollbackTransaction(cancellationToken);
            return TagErrors.TagDelete_OperationFailed(request.Id);
        }
    }
}
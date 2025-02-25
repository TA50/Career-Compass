using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Events;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Tags.Commands.Delete;
using CareerCompass.Core.Users;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Tags;

public class DeleteTagHandlerTests
{
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();

    private readonly ILoggerAdapter<DeleteTagCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<DeleteTagCommandHandler>>();

    private readonly DeleteTagCommandHandler _sut;
    private readonly Faker _faker = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteTagHandlerTests()
    {
        _sut = new DeleteTagCommandHandler(_tagRepository, _publisher, _logger);
    }

    [Fact]
    public async Task ShouldSuccessfullyDeleteTag_WhenTagExists()
    {
        // Arrange

        var tag = Tag.Create(UserId.CreateUnique(), _faker.Random.AlphaNumeric(10));
        var spec = new GetTagByIdSpecification(tag.Id, tag.UserId);
        _tagRepository.Exists(spec, _cancellationToken).Returns(true);
        _tagRepository.Delete(tag.Id, _cancellationToken).Returns(new RepositoryResult());
        _publisher.Publish(Arg.Is<TagDeletedEvent>(e => e.TagId == tag.Id), _cancellationToken)
            .Returns(Task.CompletedTask);

        var request = new DeleteTagCommand(tag.UserId, tag.Id);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        await _publisher.Received(1).Publish(Arg.Is<TagDeletedEvent>(e => e.TagId == tag.Id), _cancellationToken);
        await _tagRepository.Received(1).Delete(tag.Id, _cancellationToken);
        _logger.Received(1)
            .LogInformation("Deleting tag with id: {TagId} for user: {UserId}", tag.Id, tag.UserId);
        _logger.Received(1)
            .LogInformation("Tag with id: {TagId} for user: {UserId} deleted", tag.Id, tag.UserId);
    }


    [Fact]
    public async Task ShouldReturnTagNotFoundError_WhenTagDoesNotExists()
    {
        // Arrange

        var tagId = TagId.CreateUnique();
        var userId = UserId.CreateUnique();
        var spec = new GetTagByIdSpecification(tagId, userId);
        _tagRepository.Exists(spec, _cancellationToken).Returns(false);
        var request = new DeleteTagCommand(userId, tagId);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(TagErrors.TagDelete_TagNotFound(tagId));

        _logger.Received(1).LogInformation("Deleting tag with id: {TagId} for user: {UserId}", tagId, userId);
    }

    [Fact]
    public async Task ShouldReturnDeleteOperationFailedError_WhenDbFails()
    {
        // Arrange
        var tagId = TagId.CreateUnique();
        var userId = UserId.CreateUnique();
        var spec = new GetTagByIdSpecification(tagId, userId);
        _tagRepository.Exists(spec, _cancellationToken).Returns(true);
        const string errorMessage = "Error";
        _tagRepository.Delete(tagId, _cancellationToken).Returns(new RepositoryResult(errorMessage));
        var request = new DeleteTagCommand(userId, tagId);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(TagErrors.TagDelete_OperationFailed(tagId));


        _logger.Received(1).LogInformation("Deleting tag with id: {TagId} for user: {UserId}", tagId, userId);
        _logger.Received(1).LogError(
            "Failed to delete tag with id: {TagId} for user: {UserId}. Error: {ErrorMessage}",
            tagId, userId, errorMessage);

        await _tagRepository.Received(1).Delete(tagId, _cancellationToken);
    }

    [Fact]
    public async Task ShouldReturnDeleteOperationFailedError_WhenPublishingFails()
    {
        // Arrange
        var tagId = TagId.CreateUnique();
        var userId = UserId.CreateUnique();
        var spec = new GetTagByIdSpecification(tagId, userId);
        _tagRepository.Exists(spec, _cancellationToken).Returns(true);
        const string errorMessage = "Error";
        _tagRepository.Delete(tagId, _cancellationToken).Returns(new RepositoryResult());

        _publisher.When(x => x.Publish(Arg.Is<TagDeletedEvent>(e => e.TagId == tagId), _cancellationToken))
            .Throw(new Exception(errorMessage));

        var request = new DeleteTagCommand(userId, tagId);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(TagErrors.TagDelete_OperationFailed(tagId));


        _logger.Received(1).LogInformation("Deleting tag with id: {TagId} for user: {UserId}", tagId, userId);
        _logger.Received(1).LogError(Arg.Is<Exception>(e => e.Message == errorMessage),
            "Failed to publish event: {EventName}", nameof(TagDeletedEvent));

        await _tagRepository.Received(1).Delete(tagId, _cancellationToken);
    }
}
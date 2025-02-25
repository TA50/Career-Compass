using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Events;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Fields.Commands.DeleteField;
using CareerCompass.Core.Users;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Fields;

public class DeleteFieldHandlerTests
{
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();

    private readonly ILoggerAdapter<DeleteFieldCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<DeleteFieldCommandHandler>>();

    private readonly DeleteFieldCommandHandler _sut;
    private readonly Faker _faker = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteFieldHandlerTests()
    {
        _sut = new DeleteFieldCommandHandler(_fieldRepository, _publisher, _logger);
    }

    [Fact]
    public async Task ShouldSuccessfullyDeleteField_WhenFieldExists()
    {
        // Arrange

        var field = Field.Create(UserId.CreateUnique(), _faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(10));
        var spec = new GetFieldByIdSpecification(field.Id, field.UserId);
        _fieldRepository.Exists(spec, _cancellationToken).Returns(true);
        _fieldRepository.Delete(field.Id, _cancellationToken).Returns(new RepositoryResult());
        _publisher.Publish(Arg.Is<FieldDeletedEvent>(e => e.FieldId == field.Id), _cancellationToken)
            .Returns(Task.CompletedTask);

        var request = new DeleteFieldCommand(field.UserId, field.Id);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeFalse();

        await _publisher.Received(1).Publish(Arg.Is<FieldDeletedEvent>(e => e.FieldId == field.Id), _cancellationToken);
        await _fieldRepository.Received(1).Delete(field.Id, _cancellationToken);
        _logger.Received(1)
            .LogInformation("Deleting field with id: {FieldId} for user: {UserId}", field.Id, field.UserId);
        _logger.Received(1)
            .LogInformation("Field with id: {FieldId} for user: {UserId} deleted", field.Id, field.UserId);
    }


    [Fact]
    public async Task ShouldReturnFieldNotFoundError_WhenFieldDoesNotExists()
    {
        // Arrange

        var fieldId = FieldId.CreateUnique();
        var userId = UserId.CreateUnique();
        var spec = new GetFieldByIdSpecification(fieldId, userId);
        _fieldRepository.Exists(spec, _cancellationToken).Returns(false);
        var request = new DeleteFieldCommand(userId, fieldId);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(FieldErrors.FieldDelete_FieldNotFound(fieldId));

        _logger.Received(1).LogInformation("Deleting field with id: {FieldId} for user: {UserId}", fieldId, userId);
    }

    [Fact]
    public async Task ShouldReturnDeleteOperationFailedError_WhenDbFails()
    {
        // Arrange
        var fieldId = FieldId.CreateUnique();
        var userId = UserId.CreateUnique();
        var spec = new GetFieldByIdSpecification(fieldId, userId);
        _fieldRepository.Exists(spec, _cancellationToken).Returns(true);
        const string errorMessage = "Error";
        _fieldRepository.Delete(fieldId, _cancellationToken).Returns(new RepositoryResult(errorMessage));
        var request = new DeleteFieldCommand(userId, fieldId);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(FieldErrors.FieldDelete_OperationFailed(fieldId));


        _logger.Received(1).LogInformation("Deleting field with id: {FieldId} for user: {UserId}", fieldId, userId);
        _logger.Received(1).LogError(
            "Failed to delete field with id: {FieldId} for user: {UserId}. Error: {ErrorMessage}",
            fieldId, userId, errorMessage);

        await _fieldRepository.Received(1).Delete(fieldId, _cancellationToken);
    }

    [Fact]
    public async Task ShouldReturnDeleteOperationFailedError_WhenPublishingFails()
    {
        // Arrange
        var fieldId = FieldId.CreateUnique();
        var userId = UserId.CreateUnique();
        var spec = new GetFieldByIdSpecification(fieldId, userId);
        _fieldRepository.Exists(spec, _cancellationToken).Returns(true);
        const string errorMessage = "Error";
        _fieldRepository.Delete(fieldId, _cancellationToken).Returns(new RepositoryResult());

        _publisher.When(x => x.Publish(Arg.Is<FieldDeletedEvent>(e => e.FieldId == fieldId), _cancellationToken))
            .Throw(new Exception(errorMessage));

        var request = new DeleteFieldCommand(userId, fieldId);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(FieldErrors.FieldDelete_OperationFailed(fieldId));


        _logger.Received(1).LogInformation("Deleting field with id: {FieldId} for user: {UserId}", fieldId, userId);
        _logger.Received(1).LogError(Arg.Is<Exception>(e => e.Message == errorMessage),
            "Failed to publish event: {EventName}", nameof(FieldDeletedEvent));

        await _fieldRepository.Received(1).Delete(fieldId, _cancellationToken);
    }
}
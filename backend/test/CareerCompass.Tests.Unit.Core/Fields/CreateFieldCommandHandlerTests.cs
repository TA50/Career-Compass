using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Fields.Commands.CreateField;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;


namespace CareerCompass.Tests.Unit.Core.Fields;

public class CreateFieldCommandHandlerTests
{
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<CreateFieldCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<CreateFieldCommandHandler>>();

    private readonly CreateFieldCommandHandler _sut;

    public CreateFieldCommandHandlerTests()
    {
        _sut = new CreateFieldCommandHandler(_fieldRepository, _userRepository, _logger);
    }

    [Fact(DisplayName = "Handle: SHOULD return FieldValidation_UserNotFound Error WHEN user was not found")]
    public async Task Handle__ShouldReturnFieldValidation_UserNotFound__WhenUserWasNotFound()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string fieldName = "test field name";
        const string groupName = "test group name";
        var createFieldInput = new CreateFieldCommand(userId, fieldName, groupName);

        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(false);
        // Act: 
        var result = await _sut.Handle(createFieldInput, CancellationToken.None);

        // Assert:

        _logger.Received(1).LogInformation("Creating field {Name} inside group {Group} for user {UserId}", fieldName,
            groupName, userId);

        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError
            .ShouldBeEquivalentTo(FieldErrors.FieldValidation_UserNotFound(userId));
    }

    [Fact(DisplayName = "Handle: SHOULD return FieldValidation_NameAlreadyExists Error WHEN field name already exists")]
    public async Task Handle__ShouldReturnFieldValidation_NameAlreadyExistsError__WhenFieldNameAlreadyExists()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string fieldName = "test field name";
        const string groupName = "test group name";
        var errorMessage = "Database error";
        var createFieldInput = new CreateFieldCommand(userId, fieldName, groupName);
        var spec = new GetUserFieldByNameAndGroupSpecification(userId, fieldName, groupName);

        _fieldRepository.Exists(spec, Arg.Any<CancellationToken>())
            .Returns(true);

        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);

        // Act: 
        var result = await _sut.Handle(createFieldInput, CancellationToken.None);

        // Assert:
        _logger.Received(1).LogInformation("Creating field {Name} inside group {Group} for user {UserId}", fieldName,
            groupName, userId);

        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError
            .ShouldBeEquivalentTo(FieldErrors.FieldValidation_NameAlreadyExists(userId, fieldName, groupName));
    }

    [Fact(DisplayName = "Handle: SHOULD return Field WHEN CreateFieldInput is valid")]
    public async Task Handle__ShouldReturnField_WhenCreateFieldInputIsValid()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string fieldName = "test field name";
        const string groupName = "test group name";
        var createFieldInput = new CreateFieldCommand(userId, fieldName, groupName);
        var spec = new GetUserFieldByNameAndGroupSpecification(userId, fieldName, groupName);

        _fieldRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);

        var createdField = Field.Create(userId, fieldName, groupName);

        _fieldRepository.Create(
                Arg.Do<Field>(x => createdField = x),
                Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult());

        // Act: 
        var result = await _sut.Handle(createFieldInput, CancellationToken.None);

        // Assert:
        _logger.Received(1).LogInformation("Creating field {Name} inside group {Group} for user {UserId}", fieldName,
            groupName, userId);

        _logger.Received(1).LogInformation("Created field {Name} inside group {Group} for user {UserId}", fieldName,
            groupName, userId);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(createdField);
    }

    [Fact]
    public async Task Handle_ShouldLogErrorMessage_WhenDatabaseOperationFailed()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string fieldName = "test field name";
        const string groupName = "test group name";
        var createFieldInput = new CreateFieldCommand(userId, fieldName, groupName);
        var spec = new GetUserFieldByNameAndGroupSpecification(userId, fieldName, groupName);
        _fieldRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);

        var createdField = Field.Create(userId, fieldName, groupName);

        const string errorMessage = "Database error";

        _fieldRepository.Create(
                Arg.Do<Field>(x => createdField = x),
                Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult(errorMessage));

        // Act: 
        var result = await _sut.Handle(createFieldInput, CancellationToken.None);

        // Assert:
        _logger.Received(1).LogInformation("Creating field {Name} inside group {Group} for user {UserId}", fieldName,
            groupName, userId);


        _logger.Received(1).LogError(
            "Failed to create field {Name} inside group {Group} for user {UserId}. Failed because: {Message}",
            fieldName,
            groupName, userId, errorMessage);

        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.ShouldBeEquivalentTo(FieldErrors.FieldCreation_FailedToCreateField(
            userId, fieldName, groupName
        ));
    }
}
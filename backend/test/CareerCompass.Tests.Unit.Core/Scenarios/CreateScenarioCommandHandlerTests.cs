using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Commands.CreateScenario;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class CreateScenarioCommandHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<CreateScenarioCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<CreateScenarioCommandHandler>>();

    private readonly CreateScenarioCommandHandler _sut;

    public CreateScenarioCommandHandlerTests()
    {
        _sut = new(
            _scenarioRepository,
            _tagRepository,
            _fieldRepository,
            _userRepository,
            _logger
        );
    }


    [Fact(DisplayName = "Handle: Should return created scenario when input is valid")]
    public async Task Handle_ShouldReturnCreatedScenario_WhenInputIsValid()
    {
        // Arrange
        var tagId = TagId.CreateUnique();
        var fieldId = FieldId.CreateUnique();
        var title = "Test Scenario";
        var userId = UserId.CreateUnique();

        var request = new CreateScenarioCommand(
            Title: title,
            TagIds: [tagId],
            ScenarioFields:
            [
                new(fieldId, "Test Field 1"),
            ],
            UserId: userId,
            Date: DateTime.UtcNow
        );
        var expectedScenario = Scenario.Create(
            title: request.Title,
            userId: request.UserId,
            date: request.Date
        );

        _tagRepository.Exists(new GetTagByIdSpecification(tagId, request.UserId), Arg.Any<CancellationToken>())
            .Returns(true);
        _fieldRepository.Exists(new GetFieldByIdSpecification(fieldId, request.UserId), Arg.Any<CancellationToken>())
            .Returns(true);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Create(Arg.Do<Scenario>(x => expectedScenario = x), Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult());

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        _logger.Received(1).LogInformation("Creating scenario for user {@UserId} {@ScenarioTitle}", request.UserId,
            request.Title);
        _logger.Received(1).LogInformation("Scenario created successfully with id: {@ScenarioId}", expectedScenario.Id);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(expectedScenario);
    }

    [Fact(DisplayName = "Handle: Should return ScenarioCreation_TagNotFound errors when tags do not exist")]
    public async Task Handle__ShouldReturnScenarioCreation_TagNotFound_WhenTagDontExist()
    {
        // Arrange
        var inValidTagId = TagId.CreateUnique();
        var secondInValidTagId = TagId.CreateUnique();
        var validTagId = TagId.CreateUnique();
        const string title = "Test Scenario";
        var userId = UserId.CreateUnique();

        var input = new CreateScenarioCommand(
            Title: title,
            TagIds: [inValidTagId, validTagId, secondInValidTagId],
            ScenarioFields: [],
            UserId: userId,
            Date: DateTime.UtcNow
        );

        _tagRepository.Exists(new GetTagByIdSpecification(validTagId, userId), Arg.Any<CancellationToken>())
            .Returns(true);
        _tagRepository.Exists(new GetTagByIdSpecification(inValidTagId, userId), Arg.Any<CancellationToken>())
            .Returns(false);
        _tagRepository.Exists(new GetTagByIdSpecification(secondInValidTagId, userId), Arg.Any<CancellationToken>())
            .Returns(false);
        _userRepository.Exists(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(Arg.Any<FieldId>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(input, CancellationToken.None);


        // Assert
        _logger.Received(1).LogInformation("Creating scenario for user {@UserId} {@ScenarioTitle}", userId,
            title);


        result.IsError.Should().BeTrue();
        const int expectedCount = 2;
        result.Errors.Should().HaveCount(expectedCount);
        result.Errors.ShouldContainEquivalentOf(ScenarioErrors.ScenarioCreation_TagNotFound(inValidTagId));
        result.Errors.ShouldContainEquivalentOf(ScenarioErrors.ScenarioCreation_TagNotFound(secondInValidTagId));
        result.Errors.Count.Should().Be(2); // 2 errors
    }

    [Fact(DisplayName = "Handle: Should return ScenarioCreation_FieldNotFound errors when fields do not exist")]
    public async Task Handle__ShouldReturnScenarioCreation_FieldNotFound_WhenFieldsDontExist()
    {
        // Arrange
        var inValidFieldId = FieldId.CreateUnique();
        var secondInValidFieldId = FieldId.CreateUnique();
        var validFieldId = FieldId.CreateUnique();
        const string title = "Test Scenario";

        var userId = UserId.CreateUnique();
        var tagId = TagId.CreateUnique();
        var request = new CreateScenarioCommand(
            Title: title,
            TagIds: [tagId],
            ScenarioFields:
            [
                new(inValidFieldId, "Test Field 1"),
                new(validFieldId, "Test Field 2"),
                new(secondInValidFieldId, "Test Field 3"),
            ],
            UserId: userId,
            Date: DateTime.UtcNow
        );


        var spec = new GetFieldByIdSpecification(inValidFieldId, request.UserId);
        _fieldRepository.Exists(new GetFieldByIdSpecification(validFieldId, request.UserId),
                Arg.Any<CancellationToken>())
            .Returns(true);
        _fieldRepository
            .Exists(new GetFieldByIdSpecification(inValidFieldId, request.UserId), Arg.Any<CancellationToken>())
            .Returns(false);
        _fieldRepository.Exists(new GetFieldByIdSpecification(secondInValidFieldId, request.UserId),
            Arg.Any<CancellationToken>()).Returns(false);

        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _tagRepository.Exists(new GetTagByIdSpecification(tagId, userId), Arg.Any<CancellationToken>()).Returns(true);


        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Creating scenario for user {@UserId} {@ScenarioTitle}", request.UserId,
            request.Title);

        result.IsError.Should().BeTrue();
        result.Errors.ShouldContainEquivalentOf(ScenarioErrors.ScenarioCreation_FieldNotFound(inValidFieldId));
        result.Errors.ShouldContainEquivalentOf(ScenarioErrors.ScenarioCreation_FieldNotFound(secondInValidFieldId));
        result.Errors.Count.Should().Be(2); // 2 errors
    }


    [Fact(DisplayName =
        "Handle: Should return `ScenarioCreation_UserNotFound` Error when user does not exist")]
    public async Task Handle__ShouldReturnScenarioCreation_UserNotFoundError__WhenUserDoesntExist()
    {
        var invalidUserId = UserId.CreateUnique();
        var tagId = TagId.CreateUnique();
        const string title = "Test Scenario User Not Found";
        var request = new CreateScenarioCommand(
            Title: title, TagIds: [], ScenarioFields: [],
            UserId: invalidUserId,
            Date: DateTime.UtcNow
        );

        _userRepository.Exists(invalidUserId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        _logger.Received(1).LogInformation("Creating scenario for user {@UserId} {@ScenarioTitle}", request.UserId,
            request.Title);

        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioCreation_UserNotFound(invalidUserId));
    }


    [Fact(DisplayName = "Handle: Should return ScenarioCreationFailed error when db fails")]
    public async Task Handle_ShouldReturnScenarioCreationFailedError_WhenDbFails()
    {
        // Arrange
        var tagId = TagId.CreateUnique();
        var fieldId = FieldId.CreateUnique();
        var title = "Test Scenario";
        var userId = UserId.CreateUnique();

        var request = new CreateScenarioCommand(
            Title: title,
            TagIds: [tagId],
            ScenarioFields:
            [
                new(fieldId, "Test Field 1"),
            ],
            UserId: userId,
            Date: DateTime.UtcNow
        );
        var expectedScenario = Scenario.Create(
            title: request.Title,
            userId: request.UserId,
            date: request.Date
        );

        _tagRepository.Exists(new GetTagByIdSpecification(tagId, request.UserId), Arg.Any<CancellationToken>())
            .Returns(true);
        _fieldRepository.Exists(new GetFieldByIdSpecification(fieldId, request.UserId), Arg.Any<CancellationToken>())
            .Returns(true);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);

        const string dbError = "DB Error";
        _scenarioRepository.Create(Arg.Do<Scenario>(x => expectedScenario = x), Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult(dbError));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        _logger.Received(1).LogInformation("Creating scenario for user {@UserId} {@ScenarioTitle}", request.UserId,
            request.Title);

        _logger.LogError("Failed to create scenario with title: {@ScenarioTitle}. Reason: {@message}", request.Title,
            dbError);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioCreation_CreationFailed(request.Title));
        result.Errors.Should().HaveCount(1);
    }
}
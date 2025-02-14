using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Commands.UpdateScenario;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;


namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class UpdateScenarioCommandHandlerTests

{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<UpdateScenarioCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<UpdateScenarioCommandHandler>>();

    private readonly UpdateScenarioCommandHandler _sut;

    public UpdateScenarioCommandHandlerTests()
    {
        _sut = new UpdateScenarioCommandHandler(
            _scenarioRepository,
            _tagRepository,
            _fieldRepository,
            _userRepository, _logger
        );
    }


    [Fact(DisplayName = "Handle: Should return updated scenario when input is valid",
        Skip = "Use case is Not implemented")]
    public async Task Handle_ShouldReturnUpdatedScenario_WhenInputIsValid()
    {
        // Arrange
        var tagId = TagId.CreateUnique();
        var fieldId = FieldId.CreateUnique();
        var title = "Test Scenario";
        var userId = UserId.CreateUnique();
        var scenarioId = ScenarioId.CreateUnique();
        var input = new UpdateScenarioCommand(
            Id: scenarioId,
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
            title: input.Title,
            userId: input.UserId,
            date: input.Date
        );


        _tagRepository.Exists(new GetTagByIdSpecification(tagId, userId), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(new GetFieldByIdSpecification(fieldId, userId), Arg.Any<CancellationToken>())
            .Returns(true);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Get(expectedScenario.Id, true, Arg.Any<CancellationToken>())
            .Returns(expectedScenario);

        // Act
        var result = await _sut.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        // result.Value.Should().BeEquivalentTo(expectedScenario);
    }

    [Fact(DisplayName = "Handle: Should return ScenarioModification_TagNotFound errors when tags do not exist",
        Skip = "Use case is Not implemented")]
    public async Task Handle__ShouldReturnScenarioModification_TagNotFound_WhenTagDontExist()
    {
        // Arrange
        var inValidTagId = TagId.CreateUnique();
        var secondInValidTagId = TagId.CreateUnique();
        var validTagId = TagId.CreateUnique();
        const string title = "Test Scenario";
        var scenarioId = ScenarioId.CreateUnique();
        var input = new UpdateScenarioCommand(
            Id: scenarioId,
            Title: title,
            TagIds: [inValidTagId, validTagId, secondInValidTagId],
            ScenarioFields: [],
            UserId: UserId.CreateUnique(),
            Date: DateTime.UtcNow
        );

        _tagRepository.Exists(Arg.Is(validTagId), Arg.Any<CancellationToken>()).Returns(true);
        _tagRepository.Exists(Arg.Is(inValidTagId), Arg.Any<CancellationToken>()).Returns(false);
        _tagRepository.Exists(Arg.Is(secondInValidTagId), Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Exists(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(Arg.Any<FieldId>(), Arg.Any<CancellationToken>()).Returns(true);


        // Act
        var result = await _sut.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainEquivalentOf(ScenarioErrors.ScenarioModification_TagNotFound(inValidTagId));
        result.Errors.Should().ContainEquivalentOf(ScenarioErrors.ScenarioModification_TagNotFound(secondInValidTagId));
        result.Errors.Should().HaveCount(2); // 2 errors
    }

    [Fact(DisplayName = "Handle: Should return ScenarioModification_FieldNotFound errors when fields do not exist",
        Skip = "Use case is Not implemented")]
    public async Task Handle__ShouldReturnScenarioModification_FieldNotFound_WhenFieldsDontExist()
    {
        // Arrange
        var inValidFieldId = FieldId.CreateUnique();
        var secondInValidFieldId = FieldId.CreateUnique();
        var validFieldId = FieldId.CreateUnique();
        const string title = "Test Scenario";
        var userId = UserId.CreateUnique();
        var scenarioId = ScenarioId.CreateUnique();
        var input = new UpdateScenarioCommand(
            Id: scenarioId,
            Title: title,
            TagIds: [],
            ScenarioFields:
            [
                new(inValidFieldId, "Test Field 1"),
                new(validFieldId, "Test Field 2"),
                new(secondInValidFieldId, "Test Field 3"),
            ],
            UserId: userId,
            Date: DateTime.UtcNow
        );


        _fieldRepository.Exists(new GetFieldByIdSpecification(validFieldId, userId), Arg.Any<CancellationToken>())
            .Returns(true);
        _fieldRepository.Exists(new GetFieldByIdSpecification(inValidFieldId, userId), Arg.Any<CancellationToken>())
            .Returns(false);
        _fieldRepository
            .Exists(new GetFieldByIdSpecification(secondInValidFieldId, userId), Arg.Any<CancellationToken>())
            .Returns(false);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);


        // Act
        var result = await _sut.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainEquivalentOf(ScenarioErrors.ScenarioModification_FieldNotFound(inValidFieldId));
        result.Errors.Should()
            .ContainEquivalentOf(ScenarioErrors.ScenarioModification_FieldNotFound(secondInValidFieldId));
        result.Errors.Count.Should().Be(2); // 2 errors
    }


    [Fact(DisplayName =
            "Handle: Should return `ScenarioModification_UserNotFound` Error when user does not exist",
        Skip = "Use case is Not implemented")]
    public async Task Handle__ShouldReturnScenarioModification_UserNotFoundError__WhenUserDoesntExist()
    {
        var invalidUserId = UserId.CreateUnique();
        const string title = "Test Scenario User Not Found";
        var scenarioId = ScenarioId.CreateUnique();
        var input = new UpdateScenarioCommand(
            Id: scenarioId,
            Title: title, TagIds: [], ScenarioFields: [],
            UserId: invalidUserId,
            Date: DateTime.UtcNow
        );


        _tagRepository.Exists(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(Arg.Any<FieldId>(), Arg.Any<CancellationToken>()).Returns(true);
        _userRepository.Exists(invalidUserId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioModification_UserNotFound(invalidUserId));
    }
}
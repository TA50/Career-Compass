using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.Commands.CreateScenario;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using CareerCompass.Tests.Unit.Common;
using Moq;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Scenarios;

/**
     * Given a scenario input
     * When Input Tag exists, and Fields exist
     * Then Scenario is created
     */
public class CreateScenarioTests
{
    private readonly Mock<IScenarioRepository> _mockScenarioRepository;
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly Mock<IFieldRepository> _mockFieldRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateScenarioCommandHandler _commandHandler;

    public CreateScenarioTests()
    {
        _mockScenarioRepository = new Mock<IScenarioRepository>();
        _mockTagRepository = new Mock<ITagRepository>();
        _mockFieldRepository = new Mock<IFieldRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _commandHandler = new CreateScenarioCommandHandler(
            _mockScenarioRepository.Object,
            _mockTagRepository.Object,
            _mockFieldRepository.Object,
            _mockUserRepository.Object
        );
    }

    /**
     * Input is Valid (Success)
     */
    [Fact]
    public async Task GivenValidScenarioInput_ThenScenarioIsCreated()
    {
        // Arrange
        var tagId = TagId.NewId();
        var fieldId = FieldId.NewId();
        var title = "Test Scenario";
        var userId = UserId.NewId();

        var input = new CreateScenarioCommand(
            Title: title,
            TagIds: [tagId],
            ScenarioFields:
            [
                new(fieldId.ToString(), "Test Field 1"),
            ],
            UserId: userId,
            Date: DateTime.UtcNow
        );

        _mockTagRepository.Setup(repo => repo.Exists(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(repo => repo.Exists(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockFieldRepository.Setup(repo =>
            repo.Exists(fieldId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var createdScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: input.Title,
            tagIds: input.TagIds.Select(a => new TagId(a)).ToList(),
            scenarioFields: input.ScenarioFields.Select(
                sf => new ScenarioField(sf.FieldId, sf.Value)
            ).ToList(),
            userId: input.UserId,
            date: input.Date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(It.IsNotNull<Scenario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _commandHandler.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(createdScenario);
    }

    /**
     * Input is Invalid (Tag does not exist)
     */
    [Fact(DisplayName =
        "Given Invalid Scenario Input With Non Existent Tags, Then Scenario Is `ScenarioValidation_TagNotFound` Errors Are Returned")]
    public async Task GivenInvalidScenarioInputWithNonExistentTag_ThenScenarioIsNotCreatedAndErrorIsReturned()
    {
        // Arrange
        var inValidTagId = TagId.NewId();
        var secondInValidTagId = TagId.NewId();
        var validTagId = TagId.NewId();
        var title = "Test Scenario";

        var input = new CreateScenarioCommand(
            Title: title,
            TagIds: [inValidTagId, validTagId],
            ScenarioFields:
            [
                new(FieldId.NewId().ToString(), "Test Field 1"),
                new(FieldId.NewId().ToString(), "Test Field 2"),
            ],
            UserId: UserId.NewId(),
            Date: DateTime.UtcNow
        );


        _mockTagRepository.Setup(repo => repo.Exists(validTagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTagRepository.Setup(repo =>
                repo.Exists(It.IsIn(inValidTagId, secondInValidTagId),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _mockFieldRepository.Setup(repo => repo.Exists(It.IsAny<FieldId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(repo => repo.Exists(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createdScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: input.Title,
            tagIds: input.TagIds.Select(a => new TagId(a)).ToList(),
            scenarioFields: input.ScenarioFields.Select(
                sf => new ScenarioField(sf.FieldId, sf.Value)
            ).ToList(),
            userId: input.UserId,
            date: input.Date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(createdScenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _commandHandler.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(ScenarioError.ScenarioCreation_TagNotFound(inValidTagId));
    }

    /**
     * Input is Invalid (Field does not exist)
     */
    [Fact(DisplayName =
        "Given Invalid Scenario Input With Non Existent Field, Then Scenario Is Not Created And `ScenarioValidation_FieldNotFound` Error Is Returned")]
    public async Task GivenInvalidScenarioInputWithNonExistentField_ThenScenarioIsNotCreatedAndErrorIsReturned()
    {
        var inValidFieldId = FieldId.NewId();
        var secondInValidFieldId = FieldId.NewId();
        var validFieldId = FieldId.NewId();

        var title = "Test Scenario Field Not Found";
        var input = new CreateScenarioCommand(
            Title: title, TagIds: [TagId.NewId()], ScenarioFields:
            [
                new(inValidFieldId.ToString(), "Test Field 1"),
                new(secondInValidFieldId.ToString(), "Test Field 2"),
                new(validFieldId.ToString(), "Test Field 3")
            ],
            UserId: UserId.NewId(),
            Date: DateTime.UtcNow
        );

        _mockTagRepository.Setup(repo => repo.Exists(It.IsAny<TagId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(repo => repo.Exists(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockFieldRepository.Setup(repo => repo.Exists(validFieldId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        _mockFieldRepository.Setup(repo =>
                repo.Exists(It.IsIn(inValidFieldId, secondInValidFieldId),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: input.Title,
            tagIds: input.TagIds.Select(a => new TagId(a)).ToList(),
            scenarioFields: input.ScenarioFields.Select(
                sf => new ScenarioField(sf.FieldId, sf.Value)
            ).ToList(),
            userId: input.UserId,
            date: input.Date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(createdScenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _commandHandler.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(ScenarioError.ScenarioValidation_FieldNotFound(inValidFieldId));
        result.Errors.ShouldContainError(ScenarioError.ScenarioValidation_FieldNotFound(secondInValidFieldId));
    }

    /**
     * Input is Invalid (User does not exist)
     */
    [Fact(DisplayName =
        "Given Invalid Scenario Input With Non Existent User, Then Scenario Is Not Created And `ScenarioValidation_UserNotFound` Error Is Returned")]
    public async Task GivenInvalidScenarioInputWithNonExistentUser_ThenScenarioIsNotCreatedAndErrorIsReturned()
    {
        var invalidUserId = UserId.NewId();
        var title = "Test Scenario User Not Found";
        var input = new CreateScenarioCommand(
            Title: title, TagIds: [TagId.NewId()], ScenarioFields:
            [
                new(FieldId.NewId().ToString(), "Test Field 1"),
            ],
            UserId: invalidUserId,
            Date: DateTime.UtcNow
        );

        _mockTagRepository.Setup(repo => repo.Exists(It.IsAny<TagId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockFieldRepository.Setup(repo => repo.Exists(It.IsAny<FieldId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(repo => repo.Exists(invalidUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: input.Title,
            tagIds: input.TagIds.Select(a => new TagId(a)).ToList(),
            scenarioFields: input.ScenarioFields.Select(
                sf => new ScenarioField(sf.FieldId, sf.Value)
            ).ToList(),
            userId: input.UserId,
            date: input.Date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(createdScenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _commandHandler.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(ScenarioError.ScenarioValidation_UserNotFound(invalidUserId));
    }
}
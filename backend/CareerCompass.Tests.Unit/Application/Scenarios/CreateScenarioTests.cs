using CareerCompass.Application.Fields;
using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.UseCases;
using CareerCompass.Application.Scenarios.UseCases.Contracts;
using CareerCompass.Application.Tags;
using CareerCompass.Application.Users;
using ErrorOr;
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
    private readonly CreateScenarioUseCase _useCase;

    public CreateScenarioTests()
    {
        _mockScenarioRepository = new Mock<IScenarioRepository>();
        _mockTagRepository = new Mock<ITagRepository>();
        _mockFieldRepository = new Mock<IFieldRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _useCase = new CreateScenarioUseCase(
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

        var input = new CreateScenarioInput(
            title: title,
            tagIds: [tagId],
            scenarioFields:
            [
                new(fieldId, "Test Field 1"),
            ],
            userId: userId,
            date: DateTime.UtcNow
        );

        _mockTagRepository.Setup(repo => repo.Exists(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(repo => repo.Exists(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockFieldRepository.Setup(repo =>
            repo.Exists(fieldId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var createdScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: input.title,
            tagIds: input.tagIds,
            scenarioFields: input.scenarioFields,
            userId: input.userId,
            date: input.date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(It.IsNotNull<Scenario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _useCase.Handle(input, CancellationToken.None);

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

        var input = new CreateScenarioInput(
            title: title,
            tagIds: [inValidTagId, validTagId],
            scenarioFields:
            [
                new(FieldId.NewId(), "Test Field 1"),
                new(FieldId.NewId(), "Test Field 2"),
            ],
            userId: UserId.NewId(),
            date: DateTime.UtcNow
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
            title: input.title,
            tagIds: input.tagIds,
            scenarioFields: input.scenarioFields,
            userId: input.userId,
            date: input.date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(createdScenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(ScenarioError.ScenarioValidation_TagNotFound(inValidTagId));
        result.Errors.ShouldContain(ScenarioError.ScenarioValidation_TagNotFound(secondInValidTagId));
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
        var input = new CreateScenarioInput(
            title: title, tagIds: [TagId.NewId()], scenarioFields:
            [
                new(inValidFieldId, "Test Field 1"),
                new(secondInValidFieldId, "Test Field 2"),
                new(validFieldId, "Test Field 3")
            ],
            userId: UserId.NewId(),
            date: DateTime.UtcNow
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
            title: input.title,
            tagIds: input.tagIds,
            scenarioFields: input.scenarioFields,
            userId: input.userId,
            date: input.date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(createdScenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(ScenarioError.ScenarioValidation_FieldNotFound(inValidFieldId));
        result.Errors.ShouldContain(ScenarioError.ScenarioValidation_FieldNotFound(secondInValidFieldId));
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
        var input = new CreateScenarioInput(
            title: title, tagIds: [TagId.NewId()], scenarioFields:
            [
                new(FieldId.NewId(), "Test Field 1"),
            ],
            userId: invalidUserId,
            date: DateTime.UtcNow
        );

        _mockTagRepository.Setup(repo => repo.Exists(It.IsAny<TagId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockFieldRepository.Setup(repo => repo.Exists(It.IsAny<FieldId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(repo => repo.Exists(invalidUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createdScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: input.title,
            tagIds: input.tagIds,
            scenarioFields: input.scenarioFields,
            userId: input.userId,
            date: input.date
        );
        _mockScenarioRepository.Setup(repo => repo.Create(createdScenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdScenario);

        // Act
        var result = await _useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(ScenarioError.ScenarioValidation_UserNotFound(invalidUserId));
    }
}
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Commands.CreateScenario;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Unit.Core.Shared;
using NSubstitute;
using Shouldly;

namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class CreateScenarioCommandHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly CreateScenarioCommandHandler _sut;

    public CreateScenarioCommandHandlerTests()
    {
        _sut = new CreateScenarioCommandHandler(
            _scenarioRepository,
            _tagRepository,
            _fieldRepository,
            _userRepository
        );
    }


    [Fact(DisplayName = "Handle: Should return created scenario when input is valid")]
    public async Task Handle_ShouldReturnCreatedScenario_WhenInputIsValid()
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
        var expectedScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: input.Title,
            tagIds: input.TagIds.Select(a => new TagId(a)).ToList(),
            scenarioFields: input.ScenarioFields.Select(
                sf => new ScenarioField(sf.FieldId, sf.Value)
            ).ToList(),
            userId: input.UserId,
            date: input.Date
        );

        _tagRepository.Exists(tagId, Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(fieldId, Arg.Any<CancellationToken>()).Returns(true);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Create(Arg.Do<Scenario>(x => expectedScenario = x), Arg.Any<CancellationToken>())
            .Returns(info => info.Arg<Scenario>());

        // Act
        var result = await _sut.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEquivalentTo(expectedScenario);
    }

    [Fact(DisplayName = "Handle: Should return ScenarioCreation_TagNotFound errors when tags do not exist")]
    public async Task Handle__ShouldReturnScenarioCreation_TagNotFound_WhenTagDontExist()
    {
        // Arrange
        var inValidTagId = TagId.NewId();
        var secondInValidTagId = TagId.NewId();
        var validTagId = TagId.NewId();
        const string title = "Test Scenario";

        var input = new CreateScenarioCommand(
            Title: title,
            TagIds: [inValidTagId, validTagId, secondInValidTagId],
            ScenarioFields: [],
            UserId: UserId.NewId(),
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
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(ScenarioErrors.ScenarioCreation_TagNotFound(inValidTagId));
        result.Errors.ShouldContainError(ScenarioErrors.ScenarioCreation_TagNotFound(secondInValidTagId));
        result.Errors.Count.ShouldBe(2); // 2 errors
    }

    [Fact(DisplayName = "Handle: Should return ScenarioCreation_FieldNotFound errors when fields do not exist")]
    public async Task Handle__ShouldReturnScenarioCreation_FieldNotFound_WhenFieldsDontExist()
    {
        // Arrange
        var inValidFieldId = FieldId.NewId();
        var secondInValidFieldId = FieldId.NewId();
        var validFieldId = FieldId.NewId();
        const string title = "Test Scenario";

        var input = new CreateScenarioCommand(
            Title: title,
            TagIds: [],
            ScenarioFields:
            [
                new(inValidFieldId.ToString(), "Test Field 1"),
                new(validFieldId.ToString(), "Test Field 2"),
                new(secondInValidFieldId.ToString(), "Test Field 3"),
            ],
            UserId: UserId.NewId(),
            Date: DateTime.UtcNow
        );

        _fieldRepository.Exists(Arg.Is(validFieldId), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(Arg.Is(inValidFieldId), Arg.Any<CancellationToken>()).Returns(false);
        _fieldRepository.Exists(Arg.Is(secondInValidFieldId), Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Exists(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(true);
        _tagRepository.Exists(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns(true);


        // Act
        var result = await _sut.Handle(input, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(ScenarioErrors.ScenarioCreation_FieldNotFound(inValidFieldId));
        result.Errors.ShouldContainError(ScenarioErrors.ScenarioCreation_FieldNotFound(secondInValidFieldId));
        result.Errors.Count.ShouldBe(2); // 2 errors
    }


    [Fact(DisplayName =
        "Handle: Should return `ScenarioCreation_UserNotFound` Error when user does not exist")]
    public async Task Handle__ShouldReturnScenarioCreation_UserNotFoundError__WhenUserDoesntExist()
    {
        var invalidUserId = UserId.NewId();
        const string title = "Test Scenario User Not Found";
        var input = new CreateScenarioCommand(
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
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldHaveSingleItem();
        result.FirstError.ShouldBeEquivalentToError(ScenarioErrors.ScenarioCreation_UserNotFound(invalidUserId));
    }
}
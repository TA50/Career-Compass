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
using NSubstitute.ReturnsExtensions;


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
            _userRepository,
            _logger
        );
    }

    [Fact(DisplayName = "Handle: Should return updated scenario when input is valid")]
    public async Task Handle_ShouldReturnUpdatedScenario_WhenInputIsValid()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var tagId = TagId.CreateUnique();
        var fieldId = FieldId.CreateUnique();
        var userId = UserId.CreateUnique();
        var title = "Updated Scenario";
        var date = DateTime.UtcNow;

        var request = new UpdateScenarioCommand(
            scenarioId,
            title,
            new List<TagId> { tagId },
            new List<UpdateScenarioFieldCommand> { new(fieldId, "Updated Value") },
            userId,
            date
        );

        var scenario = Scenario.Create("Original Scenario", userId, DateTime.UtcNow);
        _scenarioRepository.Get(scenarioId, true, Arg.Any<CancellationToken>()).Returns(scenario);
        _tagRepository.Exists(Arg.Any<GetTagByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(Arg.Any<GetFieldByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(true);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult());

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating scenario {@ScenarioId}", scenarioId);
        _logger.Received(1).LogInformation("Scenario {@ScenarioId} updated successfully", scenarioId);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(scenario);
    }

    [Fact(DisplayName =
        "Handle: Should return ScenarioModification_ScenarioNotFound error when scenario does not exist")]
    public async Task Handle_ShouldReturnScenarioModification_ScenarioNotFound_WhenScenarioDoesNotExist()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var userId = UserId.CreateUnique();
        var request = new UpdateScenarioCommand(
            scenarioId,
            "Updated Scenario",
            [],
            [],
            userId,
            DateTime.UtcNow
        );

        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Get(scenarioId, true, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating scenario {@ScenarioId}", scenarioId);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioModification_ScenarioNotFound(scenarioId));
    }

    [Fact(DisplayName = "Handle: Should return ScenarioModification_ModificationFailed error when update fails")]
    public async Task Handle_ShouldReturnScenarioModification_ModificationFailed_WhenUpdateFails()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var tagId = TagId.CreateUnique();
        var fieldId = FieldId.CreateUnique();
        var userId = UserId.CreateUnique();
        var title = "Updated Scenario";
        var date = DateTime.UtcNow;
        const string errorMessage = "Update failed";

        var request = new UpdateScenarioCommand(
            scenarioId,
            title,
            new List<TagId> { tagId },
            new List<UpdateScenarioFieldCommand> { new(fieldId, "Updated Value") },
            userId,
            date
        );

        var scenario = Scenario.Create("Original Scenario", userId, DateTime.UtcNow);
        _scenarioRepository.Get(scenarioId, true, Arg.Any<CancellationToken>()).Returns(scenario);
        _tagRepository.Exists(Arg.Any<GetTagByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(Arg.Any<GetFieldByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(true);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult(errorMessage));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating scenario {@ScenarioId}", scenarioId);
        _logger.Received(1).LogError("Failed to update scenario {@Scenario}. Reason: {@message}", scenario,
            errorMessage);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioModification_ModificationFailed(title));
    }

    [Fact(DisplayName = "Handle: Should return ScenarioModification_TagNotFound error when tag does not exist")]
    public async Task Handle_ShouldReturnScenarioModification_TagNotFound_WhenTagDoesNotExist()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var tagId = TagId.CreateUnique();
        var userId = UserId.CreateUnique();
        var request = new UpdateScenarioCommand(
            scenarioId,
            "Updated Scenario",
            new List<TagId> { tagId },
            new List<UpdateScenarioFieldCommand>(),
            userId,
            DateTime.UtcNow
        );

        _tagRepository.Exists(Arg.Any<GetTagByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating scenario {@ScenarioId}", scenarioId);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioModification_TagNotFound(tagId));
    }

    [Fact(DisplayName = "Handle: Should return ScenarioModification_FieldNotFound error when field does not exist")]
    public async Task Handle_ShouldReturnScenarioModification_FieldNotFound_WhenFieldDoesNotExist()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var fieldId = FieldId.CreateUnique();
        var userId = UserId.CreateUnique();
        var request = new UpdateScenarioCommand(
            scenarioId,
            "Updated Scenario",
            new List<TagId>(),
            new List<UpdateScenarioFieldCommand> { new(fieldId, "Updated Value") },
            userId,
            DateTime.UtcNow
        );

        _tagRepository.Exists(Arg.Any<GetTagByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(true);
        _fieldRepository.Exists(Arg.Any<GetFieldByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating scenario {@ScenarioId}", scenarioId);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioModification_FieldNotFound(fieldId));
    }
}
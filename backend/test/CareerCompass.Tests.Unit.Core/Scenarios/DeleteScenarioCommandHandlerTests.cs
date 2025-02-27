using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Commands.Delete;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class DeleteScenarioCommandHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();

    private readonly ILoggerAdapter<DeleteScenarioCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<DeleteScenarioCommandHandler>>();

    private readonly DeleteScenarioCommandHandler _sut;

    public DeleteScenarioCommandHandlerTests()
    {
        _sut = new DeleteScenarioCommandHandler(_scenarioRepository, _logger);
    }

    [Fact(DisplayName = "Handle: Should return Unit when scenario is deleted successfully")]
    public async Task Handle_ShouldReturnUnit_WhenScenarioIsDeletedSuccessfully()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var userId = UserId.CreateUnique();
        var request = new DeleteScenarioCommand(scenarioId, userId);

        _scenarioRepository.Exists(Arg.Any<GetScenarioByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Delete(scenarioId, Arg.Any<CancellationToken>()).Returns(new RepositoryResult());

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Deleting scenario with id {ScenarioId}", scenarioId);
        _logger.Received(1).LogInformation("Scenario with id {ScenarioId} deleted successfully", scenarioId);

        result.IsError.Should().BeFalse();
    }

    [Fact(DisplayName = "Handle: Should return ScenarioDeletion_ScenarioNotFound error when scenario does not exist")]
    public async Task Handle_ShouldReturnScenarioDeletion_ScenarioNotFound_WhenScenarioDoesNotExist()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var userId = UserId.CreateUnique();
        var request = new DeleteScenarioCommand(scenarioId, userId);

        _scenarioRepository.Exists(Arg.Any<GetScenarioByIdSpecification>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Deleting scenario with id {ScenarioId}", scenarioId);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioDeletion_ScenarioNotFound(scenarioId));
    }

    [Fact(DisplayName = "Handle: Should return ScenarioDeletion_DeletionFailed error when deletion fails")]
    public async Task Handle_ShouldReturnScenarioDeletion_DeletionFailed_WhenDeletionFails()
    {
        // Arrange
        var scenarioId = ScenarioId.CreateUnique();
        var userId = UserId.CreateUnique();
        var request = new DeleteScenarioCommand(scenarioId, userId);
        const string errorMessage = "Deletion failed";

        _scenarioRepository.Exists(Arg.Any<GetScenarioByIdSpecification>(), Arg.Any<CancellationToken>()).Returns(true);
        _scenarioRepository.Delete(scenarioId, Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult(errorMessage));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Deleting scenario with id {ScenarioId}", scenarioId);
        _logger.Received(1).LogError("Failed to delete scenario with id {ScenarioId}. Reason: {message}", scenarioId,
            errorMessage);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioDeletion_DeletionFailed(scenarioId));
    }
}
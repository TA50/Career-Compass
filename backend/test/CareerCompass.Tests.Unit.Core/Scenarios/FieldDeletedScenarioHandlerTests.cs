using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Events;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.EventHandlers;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class FieldDeletedScenarioHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();

    private readonly ILoggerAdapter<FieldDeletedScenarioHandler> _logger =
        Substitute.For<ILoggerAdapter<FieldDeletedScenarioHandler>>();

    private readonly FieldDeletedScenarioHandler _sut;

    public FieldDeletedScenarioHandlerTests()
    {
        _sut = new FieldDeletedScenarioHandler(_scenarioRepository, _logger);
    }

    [Fact]
    public async Task ShouldSuccessfullyHandleFieldDeletedEvent()
    {
        // Arrange
        var fieldId = FieldId.CreateUnique();
        var spec = new GetScenariosSpecification();
        spec.WithFields([fieldId]);
        var scenario = Scenario.Create(
            "Test Scenario",
            UserId.CreateUnique(),
            null
        );
        scenario.AddScenarioField(fieldId, "test");

        var cancellationToken = CancellationToken.None;

        _scenarioRepository.Get(spec, true, cancellationToken).Returns([scenario]);
        _scenarioRepository.Save(cancellationToken).Returns(new RepositoryResult());
        var notification = new FieldDeletedEvent(fieldId);

        // Act

        await _sut.Handle(notification, cancellationToken);

        // Assert

        scenario.GetScenarioField(fieldId).Should().BeNull();

        _logger.Received(1).LogInformation("Handling {EventName} for field with id: {FieldId}",
            notification.GetType().Name,
            notification.FieldId);

        _logger.Received(1).LogInformation("Successfully handled {EventName} for field with id: {FieldId}",
            notification.GetType().Name,
            notification.FieldId);

        await _scenarioRepository.Received(1).Save(cancellationToken);
    }
}
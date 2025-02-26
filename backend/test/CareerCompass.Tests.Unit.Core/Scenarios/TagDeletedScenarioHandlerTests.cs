using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Events;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.EventHandlers;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class TagDeletedScenarioHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();

    private readonly ILoggerAdapter<TagDeletedScenarioHandler> _logger =
        Substitute.For<ILoggerAdapter<TagDeletedScenarioHandler>>();

    private readonly TagDeletedScenarioHandler _sut;

    public TagDeletedScenarioHandlerTests()
    {
        _sut = new TagDeletedScenarioHandler(_scenarioRepository, _logger);
    }

    [Fact]
    public async Task ShouldSuccessfullyHandleTagDeletedEvent()
    {
        // Arrange
        var tagId = TagId.CreateUnique();
        var spec = new GetScenarioHavingTagsSpecification([tagId]);
        var scenario = Scenario.Create(
            "Test Scenario",
            UserId.CreateUnique(),
            null
        );
        scenario.AddTag(tagId);

        var cancellationToken = CancellationToken.None;

        _scenarioRepository.Get(spec, true, cancellationToken).Returns([scenario]);
        _scenarioRepository.Save(cancellationToken).Returns(new RepositoryResult());
        var notification = new TagDeletedEvent(tagId);

        // Act

        await _sut.Handle(notification, cancellationToken);

        // Assert
        scenario.TagIds.Should().NotContain(tagId);
        _logger.Received(1).LogInformation("Handling {EventName} for field with id: {TagId}",
            notification.GetType().Name,
            notification.TagId);

        _logger.Received(1).LogInformation("Successfully handled {EventName} for field with id: {TagId}",
            notification.GetType().Name,
            notification.TagId);

        await _scenarioRepository.Received(1).Save(cancellationToken);
    }
}
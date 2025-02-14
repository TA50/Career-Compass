using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Queries.GetScenariosQuery;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class GetScenariosCommandHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();
    private readonly GetScenarioQueryHandler _sut;

    private readonly ILoggerAdapter<GetScenarioQueryHandler> _logger =
        Substitute.For<ILoggerAdapter<GetScenarioQueryHandler>>();

    public GetScenariosCommandHandlerTests()
    {
        _sut = new GetScenarioQueryHandler(_scenarioRepository, _logger);
    }

    [Fact(DisplayName = "Handle: Should return list of Scenarios")]
    public async Task Handle_ShouldReturnScenarios_WhenScenariosExist()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var request = new GetScenariosQuery(userId, null);
        const string title = "Test Scenario";
        var expectedScenario = Scenario.Create(
            title: title,
            userId: userId,
            date: DateTime.UtcNow
        );
        var spec = new GetUserScenariosSpecification(request.UserId);
        _scenarioRepository.Get(spec, Arg.Any<CancellationToken>()).Returns([expectedScenario]);

        // Act: 
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert:
        const int count = 1;
        _logger.Received(1).LogInformation("Getting scenarios for user {@UserId}", request.UserId);
        _logger.Received(1)
            .LogInformation("Found {ScenariosCount} scenarios for user {@UserId}", count, request.UserId);

        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(count);
        result.Value.Single().Should().BeEquivalentTo(expectedScenario);
    }

    [Fact(DisplayName = "Handle: Should return empty list when no scenario exists")]
    public async Task Handle_ShouldReturnEmptyList_WhenNoScenarioExists()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var request = new GetScenariosQuery(userId, null);
        var spec = new GetUserScenariosSpecification(request.UserId);
        _scenarioRepository.Get(spec, Arg.Any<CancellationToken>()).Returns([]);

        // Act: 
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert:
        const int count = 0;
        _logger.Received(1).LogInformation("Getting scenarios for user {@UserId}", request.UserId);
        _logger.Received(1)
            .LogInformation("Found {ScenariosCount} scenarios for user {@UserId}", count, request.UserId);
        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(count);
    }
}
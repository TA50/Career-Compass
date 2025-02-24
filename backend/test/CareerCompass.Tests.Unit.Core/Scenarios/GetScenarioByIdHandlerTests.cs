using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Scenarios;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Scenarios.Queries.GetScenarioByIdQuery;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Scenarios;

public class GetScenarioByIdHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly ILoggerAdapter<GetScenarioByIdQueryHandler> _logger =
        Substitute.For<ILoggerAdapter<GetScenarioByIdQueryHandler>>();

    private readonly GetScenarioByIdQueryHandler _sut;
    private readonly Faker _faker = new();

    public GetScenarioByIdHandlerTests()
    {
        _sut = new GetScenarioByIdQueryHandler(_scenarioRepository, _logger);
    }

    [Fact]
    public async Task ShouldReturnScenario_WhenScenarioExists()
    {
        // Arrange
        var title = _faker.Random.AlphaNumeric(10);
        var userId = UserId.CreateUnique();
        var scenario = Scenario.Create(title, userId, null);

        var spec = new GetScenarioByIdSpecification(scenario.Id, userId);
        _scenarioRepository.Single(spec, _cancellationToken).Returns(scenario);
        var request = new GetScenarioByIdQuery(scenario.Id, userId);
        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(scenario);

        await _scenarioRepository.Received(1).Single(spec, _cancellationToken);
        _logger.Received(1)
            .LogInformation("Getting scenario {ScenarioId} for user {@UserId}", request.Id, request.UserId);
        _logger.LogInformation("Got scenario {ScenarioId} for user {@UserId}", request.Id, request.UserId);
    }

    [Fact]
    public async Task ShouldReturnNotFoundError_WhenScenarioDoesNotExist()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var scenarioId = ScenarioId.CreateUnique();

        var spec = new GetScenarioByIdSpecification(scenarioId, userId);
        _scenarioRepository.Single(spec, _cancellationToken).ReturnsNull();
        var request = new GetScenarioByIdQuery(scenarioId, userId);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(ScenarioErrors.ScenarioRead_ScenarioNotFound(scenarioId));

        await _scenarioRepository.Received(1).Single(spec, _cancellationToken);
        _logger.Received(1)
            .LogInformation("Getting scenario {ScenarioId} for user {@UserId}", request.Id, request.UserId);
    }
}
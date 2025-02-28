using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Models;
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
        const int count = 1;
        var expectedScenario = Scenario.Create(
            title: title,
            userId: userId,
            date: DateTime.UtcNow
        );
        var expectedResult = new PaginationResult<Scenario>([expectedScenario], count, 1, 1);
        var spec = new GetScenariosSpecification();
        spec.BelongsTo(request.UserId);
        _scenarioRepository.Get(spec, CancellationToken.None).Returns([expectedScenario]);


        _scenarioRepository.Count(new GetScenariosSpecification(request.UserId), CancellationToken.None).Returns(count);

        // Act: 
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert:

        _logger.Received(1).LogInformation("Getting scenarios for user {@UserId}", request.UserId);
        _logger.Received(1).LogInformation(
            "Found {TotalItems} scenarios for user {UserId} on page {Page} with page size {PageSize}",
            count, request.UserId, 1, 1);
        _logger.DidNotReceive().LogInformation("No scenarios found for user {@UserId}", request.UserId);
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact(DisplayName = "Handle: Should return empty list when no scenario exists")]
    public async Task Handle_ShouldReturnEmptyList_WhenNoScenarioExists()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var request = new GetScenariosQuery(userId, null);
        var spec = new GetScenariosSpecification(request.UserId);
        _scenarioRepository.Get(spec, Arg.Any<CancellationToken>()).Returns([]);

        // Act: 
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert:
        _logger.Received(1).LogInformation("Getting scenarios for user {@UserId}", request.UserId);
        _logger.DidNotReceive().LogInformation(
            "Found {TotalItems} scenarios for user {UserId} on page {Page} with page size {PageSize}",
            Arg.Any<int>(), request.UserId, Arg.Any<int>(), Arg.Any<int>());
        _logger.Received(1).LogInformation("No scenarios found for user {@UserId}", request.UserId);
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(PaginationResult<Scenario>.Empty);
    }
}
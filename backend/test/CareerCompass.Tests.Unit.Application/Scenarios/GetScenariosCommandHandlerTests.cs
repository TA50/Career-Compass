using CareerCompass.Application.Scenarios;
using CareerCompass.Application.Scenarios.Queries.GetScenariosQuery;
using CareerCompass.Application.Users;
using NSubstitute;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Scenarios;

public class GetScenariosCommandHandlerTests
{
    private readonly IScenarioRepository _scenarioRepository = Substitute.For<IScenarioRepository>();
    private readonly GetScenarioQueryHandler _sut;

    public GetScenariosCommandHandlerTests()
    {
        _sut = new GetScenarioQueryHandler(_scenarioRepository);
    }

    [Fact(DisplayName = "Handle: Should return list of Scenarios")]
    public async Task Handle_ShouldReturnScenarios_WhenScenariosExist()
    {
        // Arrange: 


        var userId = UserId.NewId();
        var query = new GetScenariosQuery(userId, null);
        var title = "Test Scenario";

        var expectedScenario = new Scenario(
            id: ScenarioId.NewId(),
            title: title,
            tagIds: [],
            scenarioFields: [],
            userId: userId,
            date: DateTime.UtcNow
        );
        List<Scenario> scenarios = [expectedScenario];


        _scenarioRepository.Get(userId, Arg.Any<CancellationToken>()).Returns(scenarios);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldHaveSingleItem();
        result.Value.Single().ShouldBeEquivalentTo(expectedScenario);
    }

    [Fact(DisplayName = "Handle: Should return empty list when no scenario exists")]
    public async Task Handle_ShouldReturnEmptyList_WhenNoScenarioExists()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var query = new GetScenariosQuery(userId);
        _scenarioRepository.Get(userId, Arg.Any<CancellationToken>()).Returns(Enumerable.Empty<Scenario>().ToList());

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEmpty();
    }
}
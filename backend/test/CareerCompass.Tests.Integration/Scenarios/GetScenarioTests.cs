using System.Net;
using System.Net.Http.Json;
using AutoMapper;
using Bogus;
using CareerCompass.Api.Contracts.Scenarios;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Common.Models;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Tests.Integration.Scenarios;

[Collection(nameof(ApiCollection))]
public class GetScenarioTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;
    private const int ValidNameLength = 10;

    public GetScenarioTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    #region List Scenarios

    [Fact]
    public async Task ShouldGetAllUserScenarios_WhenAuthenticated()
    {
        // Arrange
        var userId = await Login();
        var scenarios = await CreateTestScenarios(userId, 3);

        // Act
        var response = await _client.GetAsync("scenarios");
        var content = await response.Content.ReadFromJsonAsync<PaginationResult<ScenarioDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        var mapper = _factory.Services.GetRequiredService<IMapper>();
        var dtos = scenarios.Select(mapper.Map<ScenarioDto>).ToList();
        var result = new PaginationResult<ScenarioDto>(dtos, 3, 10, 1);
        content.Should().BeEquivalentTo(result);

        // Cleanup

        var scenarioIds = scenarios.Select(s => s.Id);
        await _factory.DbContext.Scenarios.Where(s => scenarioIds.Contains(s.Id)).ExecuteDeleteAsync();
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldGetEmptyList_WhenUserHasNoScenarios()
    {
        // Arrange
        var userId = await Login();

        // Act
        var response = await _client.GetAsync("scenarios");
        var content = await response.Content.ReadFromJsonAsync<PaginationResult<ScenarioDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Items.Should().BeEmpty();

        // Cleanup
        await _factory.RemoveUser(userId);
    }


    [Fact]
    public async Task ShouldGetScenariosByTag_WhenTagExists()
    {
        // Arrange
        var userId = await Login();
        var tag = Tag.Create(userId, "Test Tag");
        var scenario = Scenario.Create("Test Scenario", userId, DateTime.UtcNow);
        scenario.AddTag(tag.Id);

        _factory.DbContext.Tags.Add(tag);
        _factory.DbContext.Scenarios.Add(scenario);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"scenarios?TagIds={tag.Id}");
        var content = await response.Content.ReadFromJsonAsync<PaginationResult<ScenarioDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Items.Should().HaveCount(1);
        content.Items.First().Title.Should().Be(scenario.Title);

        // Cleanup
        await _factory.DbContext.Scenarios.Where(s => s.Id == scenario.Id).ExecuteDeleteAsync();
        await _factory.DbContext.Tags.Where(t => t.Id == tag.Id).ExecuteDeleteAsync();
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldGetPaginatedScenarios_WhenPageAndPageSizeAreSpecified()
    {
        // Arrange
        var userId = await Login();
        var scenarios = await CreateTestScenarios(userId, 5);
        const int page = 1;
        const int pageSize = 2;
        // Act
        var response = await _client.GetAsync($"scenarios?Page={page}&PageSize={pageSize}");
        var content = await response.Content.ReadFromJsonAsync<PaginationResult<ScenarioDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        var mapper = _factory.Services.GetRequiredService<IMapper>();
        var expectedScenarios = await _factory.DbContext.Scenarios
            .AsNoTracking()
            .Skip((page - 1) * page)
            .Take(pageSize)
            .ToListAsync();

        var dtos = expectedScenarios.Select(mapper.Map<ScenarioDto>).ToList();
        var expected = new PaginationResult<ScenarioDto>(dtos, 5, pageSize, page);
        content.Should().BeEquivalentTo(expected);
        // Cleanup
        var scenarioIds = scenarios.Select(s => s.Id);
        await _factory.DbContext.Scenarios.Where(s => scenarioIds.Contains(s.Id)).ExecuteDeleteAsync();
        await _factory.RemoveUser(userId);
    }

    #endregion

    #region Get By ID

    [Fact]
    public async Task ShouldGetSpecificScenario_WhenScenarioExists()
    {
        // Arrange
        var userId = await Login();
        var scenario = (await CreateTestScenarios(userId, 1))[0];
        // Act
        var response = await _client.GetAsync($"scenarios/{scenario.Id}");
        var content = await response.Content.ReadFromJsonAsync<ScenarioDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Id.Should().Be(scenario.Id.ToString());
        content.Title.Should().Be(scenario.Title);
        content.Date.Should().Be(scenario.Date);

        // Cleanup
        await _factory.DbContext.Scenarios.Where(s => s.Id == scenario.Id).ExecuteDeleteAsync();
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenScenarioDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var scenarioId = Guid.Empty.ToString();
        // Act
        var response = await _client.GetAsync($"scenarios/{scenarioId}");
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        content.Should().NotBeNull();
        var expected = new ProblemDetails
        {
            Title = "Scenario Read: The requested scenario does not exist",
            Detail = $"Scenario with id {scenarioId} does not exist",
            Status = StatusCodes.Status404NotFound
        };

        expected.Extensions.Add("code", "10.10.20.20");
        expected.Extensions.Add("ScenarioId", scenarioId);
        content.ShouldBeEquivalentTo(expected);


        // Cleanup

        await _factory.RemoveUser(userId);
    }


    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("scenarios");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion


    private async Task<UserId> Login()
    {
        var password = _faker.StrongPassword();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        return user.Id;
    }

    private async Task<Scenario[]> CreateTestScenarios(UserId userId, int count)
    {
        var scenarios = new List<Scenario>();
        for (int i = 0; i < count; i++)
        {
            var s = Scenario.Create(
                _faker.Random.AlphaNumeric(ValidNameLength),
                userId,
                null);
            scenarios.Add(s);
        }

        _factory.DbContext.Scenarios.AddRange(scenarios);
        await _factory.DbContext.SaveChangesAsync();

        return scenarios.ToArray();
    }
}
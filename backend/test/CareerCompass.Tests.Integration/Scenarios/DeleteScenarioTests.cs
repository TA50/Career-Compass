using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tests.Integration.Scenarios;

[Collection(nameof(ApiCollection))]
public class DeleteScenarioTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;

    public DeleteScenarioTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ShouldDeleteScenario_WhenScenarioExists()
    {
        // Arrange
        var userId = await Login();
        var scenario = Scenario.Create("Test Scenario", userId, DateTime.UtcNow);
        _factory.DbContext.Scenarios.Add(scenario);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"scenarios/{scenario.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var deletedScenario = await _factory.DbContext.Scenarios
            .FirstOrDefaultAsync(s => s.Id == scenario.Id);
        deletedScenario.Should().BeNull();

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenScenarioDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var nonExistentScenarioId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync($"scenarios/{nonExistentScenarioId}");
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ProblemDetails
        {
            Title = "Scenario Deletion Validation: scenario with the provided id does not exist",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Scenario with id {nonExistentScenarioId} does not exist"
        };
        expected.Extensions.Add("ScenarioId", nonExistentScenarioId);
        expected.Extensions.Add("code", "10.10.40.10");
        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    private async Task<UserId> Login()
    {
        var password = new Faker().Internet.Password();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        return user.Id;
    }
}
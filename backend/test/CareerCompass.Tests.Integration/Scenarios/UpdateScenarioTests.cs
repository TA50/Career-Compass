using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Scenarios;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration.Scenarios;

[Collection(nameof(ApiCollection))]
public class UpdateScenarioTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;

    public UpdateScenarioTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ShouldUpdateScenario_WhenInputIsValid()
    {
        // Arrange
        var userId = await Login();
        var scenario = Scenario.Create("Original Scenario", userId, DateTime.UtcNow);
        var tag = Tag.Create(userId, "Original Tag");
        var field = Field.Create(userId, "Original Field", "Original Value");

        _factory.DbContext.Scenarios.Add(scenario);
        _factory.DbContext.Tags.Add(tag);
        _factory.DbContext.Fields.Add(field);
        await _factory.DbContext.SaveChangesAsync();

        var request = new UpdateScenarioRequest()
        {
            Id = scenario.Id.ToString(),
            Title = "Updated Scenario",
            TagIds = [tag.Id.ToString()],
            ScenarioFields =
            [
                new() { FieldId = field.Id.Value.ToString(), Value = "Updated Value" }
            ],
            Date = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync($"scenarios", request);

        var content = await response.Content.ReadFromJsonAsync<ScenarioDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Title.Should().Be(request.Title);
        content.ScenarioFields.First().Value.Should().Be("Updated Value");

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenScenarioDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var nonExistentScenarioId = Guid.NewGuid().ToString();

        var request = new UpdateScenarioRequest
        {
            Id = nonExistentScenarioId,
            Title = "Updated Scenario",
            TagIds = new List<string>(),
            ScenarioFields = new List<UpdateScenarioFieldRequest>(),
            Date = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync($"scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ProblemDetails
        {
            Title = "Scenario Modification Validation: The provided scenario id for this scenario does not exist",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Scenario with id {nonExistentScenarioId} does not exist"
        };
        expected.Extensions.Add("code", "10.10.30.10");
        expected.Extensions.Add("ScenarioId", nonExistentScenarioId);
        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenTagDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var scenario = Scenario.Create("Original Scenario", userId, DateTime.UtcNow);
        var nonExistentTagId = Guid.NewGuid().ToString();

        _factory.DbContext.Scenarios.Add(scenario);
        await _factory.DbContext.SaveChangesAsync();

        var request = new UpdateScenarioRequest
        {
            Id = scenario.Id.ToString(),
            Title = "Updated Scenario",
            TagIds = new List<string> { nonExistentTagId },
            ScenarioFields = new List<UpdateScenarioFieldRequest>(),
            Date = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync($"scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ProblemDetails
        {
            Title = "Scenario Modification Validation: The provided tag id for this scenario does not exist",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Tag with id {nonExistentTagId} does not exist"
        };
        expected.Extensions.Add("TagId", nonExistentTagId);
        expected.Extensions.Add("code", "10.10.30.20");
        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenFieldDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var scenario = Scenario.Create("Original Scenario", userId, DateTime.UtcNow);
        var nonExistentFieldId = Guid.NewGuid().ToString();

        _factory.DbContext.Scenarios.Add(scenario);
        await _factory.DbContext.SaveChangesAsync();

        var request = new UpdateScenarioRequest
        {
            Id = scenario.Id.ToString(),
            Title = "Updated Scenario",
            TagIds = new List<string>(),
            ScenarioFields = new List<UpdateScenarioFieldRequest>
            {
                new() { FieldId = nonExistentFieldId, Value = "Updated Value" }
            },
            Date = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync($"scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ProblemDetails
        {
            Title = "Scenario Modification Validation: The provided field id for this scenario does not exist",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Field with id {nonExistentFieldId} does not exist"
        };
        expected.Extensions.Add("code", "10.10.30.30");
        expected.Extensions.Add("FieldId", nonExistentFieldId);
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
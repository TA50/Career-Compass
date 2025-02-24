using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Scenarios;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Scenarios;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tests.Integration.Scenarios;

[Collection(nameof(ApiCollection))]
public class CreateScenarioTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;
    private const int ValidNameLength = 10;

    public CreateScenarioTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldCreateScenario_WhenInputIsValid()
    {
        // Arrange
        var userId = await Login();
        var tag = Tag.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength));
        var field = Field.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength),
            _faker.Random.AlphaNumeric(ValidNameLength));

        _factory.DbContext.Tags.Add(tag);
        _factory.DbContext.Fields.Add(field);
        await _factory.DbContext.SaveChangesAsync();

        var request = new CreateScenarioRequest
        {
            Title = _faker.Random.Words(),
            TagIds = new List<string> { tag.Id.Value.ToString() },
            ScenarioFields = new List<CreateScenarioFieldRequest>
            {
                new() { FieldId = field.Id.Value.ToString(), Value = _faker.Random.Words() }
            },
            Date = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync("scenarios", request);
        var content = await response.Content.ReadFromJsonAsync<ScenarioDto>();

        // Assert
        content.Should().NotBeNull();
        var expected = new ScenarioDto(
            Id: content.Id,
            Title: request.Title,
            Date: request.Date,
            ScenarioFields:
            [
                new(
                    FieldId: field.Id.Value.ToString(),
                    Value: request.ScenarioFields.First().Value
                )
            ]
            ,
            TagIds: [tag.Id.Value.ToString()]
        );
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        content.Should().BeEquivalentTo(expected);
        var locationHeader = response.Headers.Location;
        locationHeader.Should().NotBeNull();
        locationHeader.ToString().Should().Contain($"scenarios/{content.Id}");

        // Cleanup
        await _factory.RemoveUser(userId);
        var scenarioId = ScenarioId.Create(content.Id);
        await _factory.DbContext.Scenarios.Where(s => s.Id == scenarioId)
            .ExecuteDeleteAsync();

        await _factory.DbContext.Tags.Where(s => s.Id == tag.Id)
            .ExecuteDeleteAsync();
        await _factory.DbContext.Fields.Where(s => s.Id == field.Id)
            .ExecuteDeleteAsync();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        var userId = await Login();
        var request = new CreateScenarioRequest
        {
            Title = string.Empty,
            TagIds = [],
            ScenarioFields = []
        };

        // Act
        var response = await _client.PostAsJsonAsync("scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };
        expected.Errors.Add("Title", [
            "'Title' must not be empty."
        ]);

        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenFieldValueIsEmpty()
    {
        // Arrange
        var userId = await Login();
        var field = Field.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength),
            _faker.Random.AlphaNumeric(ValidNameLength));

        _factory.DbContext.Fields.Add(field);
        await _factory.DbContext.SaveChangesAsync();

        var request = new CreateScenarioRequest
        {
            Title = _faker.Random.AlphaNumeric(ValidNameLength),
            TagIds = [],
            ScenarioFields =
            [
                new() { FieldId = field.Id.Value.ToString(), Value = string.Empty }
            ]
        };

        // Act
        var response = await _client.PostAsJsonAsync("scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        expected.Errors.Add("ScenarioFields[0].Value", [
            "'Value' must not be empty."
        ]);
        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.DbContext.Fields.Where(f => f.Id == field.Id).ExecuteDeleteAsync();
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var nonExistentTagId = Guid.Empty.ToString();
        var request = new CreateScenarioRequest
        {
            Title = _faker.Random.AlphaNumeric(ValidNameLength),
            TagIds = [nonExistentTagId]
        };

        // Act
        var response = await _client.PostAsJsonAsync("scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ProblemDetails
        {
            Title = "Scenario Creation Validation: The provided tag id for this scenario does not exist",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Tag with id {nonExistentTagId} does not exist"
        };
        expected.Extensions.Add("TagId", nonExistentTagId);
        expected.Extensions.Add("code", "10.10.10.20");
        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFieldDoesNotExist()
    {
        // Arrange
        var userId = await Login();


        var nonExistentFieldId = Guid.Empty.ToString();
        var request = new CreateScenarioRequest
        {
            Title = _faker.Random.Words(),
            ScenarioFields = new List<CreateScenarioFieldRequest>
            {
                new() { FieldId = nonExistentFieldId, Value = _faker.Random.AlphaNumeric(ValidNameLength) }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        var expected = new ProblemDetails
        {
            Title = "Scenario Creation Validation: The provided field id for this scenario does not exist",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Field with id {nonExistentFieldId} does not exist"
        };
        expected.Extensions.Add("FieldId", nonExistentFieldId);
        expected.Extensions.Add("code", "10.10.10.30");

        problemDetails.ShouldBeEquivalentTo(expected);


        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenDuplicateFieldsExist()
    {
        // Arrange
        var userId = await Login();
        var field = Field.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength),
            _faker.Random.AlphaNumeric(ValidNameLength));
        _factory.DbContext.Fields.Add(field);
        await _factory.DbContext.SaveChangesAsync();

        var request = new CreateScenarioRequest
        {
            Title = _faker.Random.AlphaNumeric(ValidNameLength),
            ScenarioFields =
            [
                new() { FieldId = field.Id.Value.ToString(), Value = _faker.Random.AlphaNumeric(ValidNameLength) },
                new() { FieldId = field.Id.Value.ToString(), Value = _faker.Random.AlphaNumeric(ValidNameLength) }
            ]
        };

        // Act
        var response = await _client.PostAsJsonAsync("scenarios", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();

        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        expected.Errors.Add("ScenarioFields", ["Scenario Field Ids must be distinct"]);

        // Cleanup
        await _factory.RemoveUser(userId);

        await _factory.DbContext.Fields.Where(s => s.Id == field.Id)
            .ExecuteDeleteAsync();
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var request = new CreateScenarioRequest
        {
            Title = _faker.Random.Words(),
            TagIds = new List<string> { Guid.NewGuid().ToString() },
            ScenarioFields = new List<CreateScenarioFieldRequest>
            {
                new() { FieldId = Guid.NewGuid().ToString(), Value = _faker.Random.Words() }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("scenarios", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<UserId> Login()
    {
        var password = _faker.StrongPassword();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        return user.Id;
    }
}
using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Fields;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Common;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration.Fields;

[Collection(nameof(ApiCollection))]
public class CreateFieldTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    private const int ValidNameLength = 10;

    public CreateFieldTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldCreateField_WhenInputIsValid()
    {
        // Arrange

        var userId = await Login();
        var request = new CreateFieldRequest
        {
            Name = _faker.Random.AlphaNumeric(ValidNameLength),
            Group = _faker.Random.AlphaNumeric(ValidNameLength)
        };

        // Act
        var response = await _client.PostAsJsonAsync("fields", request);
        var content = await response.Content.ReadFromJsonAsync<FieldDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Should().NotBeNull();
        content!.Name.Should().Be(request.Name);
        content.Group.Should().Be(request.Group);
        content.Id.Should().NotBeEmpty();
        response.Headers.Location.Should().NotBeNull();

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenRequiredFieldsAreEmpty()
    {
        // Arrange
        var userId = await Login();
        var request = new CreateFieldRequest
        {
            Name = string.Empty,
            Group = string.Empty
        };

        // Act
        var response = await _client.PostAsJsonAsync("fields", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();

        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };
        expected.Errors.Add(nameof(CreateFieldRequest.Name), [
            "'Name' must not be empty.",
        ]);
        expected.Errors.Add(nameof(CreateFieldRequest.Group), [
            "'Group' must not be empty.",
        ]);

        problemDetails.Should().BeEquivalentTo(expected);
        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenFieldWithSameNameAndGroupExists()
    {
        // Arrange
        var userId = await Login();
        var group = _faker.Random.AlphaNumeric(ValidNameLength);
        var name = _faker.Random.AlphaNumeric(ValidNameLength);
        var request = new CreateFieldRequest
        {
            Name = name, Group = group
        };

        // Create first field
        await _client.PostAsJsonAsync("fields", request);

        // Act - Try to create field with same name and group
        var response = await _client.PostAsJsonAsync("fields", request);
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var expected = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "FieldCreation: Field with the same name already exists",
            Detail = $"Field with name '{name}' in the group: '{group}' already exists for user '{userId}'"
        };
        expected.Extensions.Add("code", "10.40.10.10");
        expected.Extensions.Add("UserId", userId.ToString());
        expected.Extensions.Add("Name", name);
        expected.Extensions.Add("Group", group);
        content.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenRequiredFieldsAreTooLong()
    {
        // Arrange
        var userId = await Login();
        var request = new CreateFieldRequest
        {
            Name = _faker.Random.AlphaNumeric(Limits.MaxNameLength + 10),
            Group = _faker.Random.AlphaNumeric(Limits.MaxNameLength + 10)
        };

        // Act
        var response = await _client.PostAsJsonAsync("fields", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();

        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };
        expected.Errors.Add(nameof(CreateFieldRequest.Name), [
            $"The length of 'Name' must be {Limits.MaxNameLength} characters or fewer. You entered {request.Name.Length} characters."
        ]);
        expected.Errors.Add(nameof(CreateFieldRequest.Group), [
            $"The length of 'Group' must be {Limits.MaxNameLength} characters or fewer. You entered {request.Group.Length} characters."
        ]);

        problemDetails.Should().BeEquivalentTo(expected);
        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var request = new CreateFieldRequest
        {
            Name = _faker.Random.AlphaNumeric(ValidNameLength),
            Group = _faker.Random.AlphaNumeric(ValidNameLength)
        };
        // Act
        var response = await _client.PostAsJsonAsync("fields", request);

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
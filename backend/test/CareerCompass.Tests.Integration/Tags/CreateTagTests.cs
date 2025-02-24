using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Tags;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Common;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration.Tags;

[Collection(nameof(ApiCollection))]
public class CreateTagTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    private const int ValidTagLength = 10;

    public CreateTagTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldCreateTag_WhenInputIsValid()
    {
        // Arrange
        var userId = await Login();

        var request = new CreateTagRequest
        {
            Name = _faker.Random.AlphaNumeric(ValidTagLength)
        };

        // Act
        var response = await _client.PostAsJsonAsync("tags", request);
        var content = await response.Content.ReadFromJsonAsync<TagDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        content.Should().NotBeNull();
        content.Name.Should().Be(request.Name);
        content.Id.Should().NotBeEmpty();
        var location = response.Headers.Location;
        location.Should().NotBeNull();
        location.AbsoluteUri.Should().Contain(content.Id);


        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var userId = await Login();

        var request = new CreateTagRequest
        {
            Name = string.Empty
        };

        // Act
        var response = await _client.PostAsJsonAsync("tags", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        problemDetails.Should().NotBeNull();

        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = (int)HttpStatusCode.BadRequest
        };

        expected.Errors.Add(nameof(CreateTagRequest.Name), [
            "'Name' must not be empty."
        ]);

        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenTagWithSameNameExists()
    {
        // Arrange
        var userId = await Login();
        var tagName = _faker.Random.AlphaNumeric(ValidTagLength);
        var tag = Tag.Create(userId, tagName);
        _factory.DbContext.Tags.Add(tag);
        await _factory.DbContext.SaveChangesAsync();

        var request = new CreateTagRequest
        {
            Name = tagName
        };

        // Create first tag
        await _client.PostAsJsonAsync("tags", request);

        // Act - Try to create tag with same name
        var response = await _client.PostAsJsonAsync("tags", request);
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var expected = new ProblemDetails
        {
            Title = "Tag Creation:Tag with the same name already exists for the user.",
            Detail = $"Tag with name {tagName} already exists for user with id {userId}",
            Status = StatusCodes.Status409Conflict
        };

        expected.Extensions.Add("code", "10.20.10.20");
        expected.Extensions.Add("TagName", tagName);
        expected.Extensions.Add("UserId", userId.ToString());

        content.Should().NotBeNull();
        content.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenTagNameIsTooLong()
    {
        var userId = await Login();

        var request = new CreateTagRequest
        {
            Name = _faker.Random.AlphaNumeric(Limits.MaxNameLength + 10)
        };

        // Act
        var response = await _client.PostAsJsonAsync("tags", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        problemDetails.Should().NotBeNull();

        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = (int)HttpStatusCode.BadRequest
        };

        expected.Errors.Add(nameof(CreateTagRequest.Name), [
            $"The length of 'Name' must be {Limits.MaxNameLength} characters or fewer. You entered {request.Name.Length} characters."
        ]);

        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Act
        var request = new CreateTagRequest
        {
            Name = _faker.Random.AlphaNumeric(ValidTagLength)
        };

        var response = await _client.PostAsJsonAsync("tags", request);


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
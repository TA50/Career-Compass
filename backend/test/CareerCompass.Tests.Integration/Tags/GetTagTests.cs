using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Tags;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration.Tags;

[Collection(nameof(ApiCollection))]
public class GetTagTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    public GetTagTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldGetAllUserTags_WhenAuthenticated()
    {
        // Arrange
        var userId = await Login();
        var tags = new List<Tag>
        {
            Tag.Create(userId, _faker.Random.AlphaNumeric(10)),
            Tag.Create(userId, _faker.Random.AlphaNumeric(10)),
            Tag.Create(userId, _faker.Random.AlphaNumeric(10))
        };
        _factory.DbContext.Tags.AddRange(tags);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("tags");
        var content = await response.Content.ReadFromJsonAsync<List<TagDto>>();

        // Assert
        var expectedTags = tags.Select(t => new TagDto(t.Id.ToString(), t.Name));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().HaveCount(3);
        content.Should().BeEquivalentTo(expectedTags);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldEmptyList_WhenNotTagExists()
    {
        // Arrange
        var userId = await Login();

        // Act
        var response = await _client.GetAsync("tags");
        var content = await response.Content.ReadFromJsonAsync<List<TagDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().BeEmpty();
        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldGetByIdTag_WhenTagExists()
    {
        // Arrange
        var userId = await Login();
        var tag = Tag.Create(userId, _faker.Random.AlphaNumeric(10));
        _factory.DbContext.Tags.Add(tag);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"tags/{tag.Id}");
        var content = await response.Content.ReadFromJsonAsync<TagDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Id.Should().Be(tag.Id.ToString());
        content.Name.Should().Be(tag.Name);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var nonExistentTagId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"tags/{nonExistentTagId}");
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        problemDetails.Should().NotBeNull();

        var expected = new ProblemDetails
        {
            Title = "Tag Read: Tag was not found ",
            Detail = $"Tag with  userId {userId} and tagId {nonExistentTagId} was not found.",
            Status = StatusCodes.Status404NotFound
        };

        expected.Extensions.Add("code", "10.20.20.10");
        expected.Extensions.Add("UserId", userId.ToString());
        expected.Extensions.Add("TagId", nonExistentTagId.ToString());

        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("tags");

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
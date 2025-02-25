using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tests.Integration.Tags;

[Collection(nameof(ApiCollection))]
public class DeleteTagTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    public DeleteTagTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldDeleteTag_WhenTagExists()
    {
        // Arrange
        var userId = await Login();
        var tag = Tag.Create(userId, _faker.Random.AlphaNumeric(10));
        _factory.DbContext.Tags.Add(tag);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"tags/{tag.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var entity = await _factory.DbContext.Tags.FirstOrDefaultAsync(f => f.Id == tag.Id);
        entity.Should().BeNull();

        // Cleanup

        await _factory.RemoveUser(userId);
    }


    [Fact]
    public async Task ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var nonExistentTagId = Guid.Empty;

        // Act
        var response = await _client.DeleteAsync($"tags/{nonExistentTagId}");
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var expected = new ProblemDetails
        {
            Title = "Tag Delete: Tag was not found ",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Tag with tagId {nonExistentTagId} was not found."
        };
        expected.Extensions.Add("code", "10.20.30.10");

        problemDetails.Should().NotBeNull();


        problemDetails.ShouldBeEquivalentTo(expected);
        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var tagId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"tags/{tagId}");

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
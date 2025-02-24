using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;

namespace CareerCompass.Tests.Integration.Users;

[Collection(nameof(ApiCollection))]
public class GetMeTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    public GetMeTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldGetUserDetails_WhenAuthenticated()
    {
        // Arrange
        var password = _faker.StrongPassword();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var response = await _client.GetAsync("users/me");
        var content = await response.Content.ReadFromJsonAsync<UserDto>();
        var expected = new UserDto(user.Id.ToString(), user.FirstName, user.LastName, user.Email);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().BeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
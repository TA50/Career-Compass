using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration.Users;

[Collection(nameof(ApiCollection))]
public class UpdateTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;


    public UpdateTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ShouldUpdateUser_WhenInputIsValid()
    {
        // Arrange
        var faker = new Faker();
        var password = faker.StrongPassword();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var request = new UpdateUserRequest(
            FirstName: faker.Person.FirstName,
            LastName: faker.Person.LastName);

        // Act
        var response = await _client.PutAsJsonAsync("users", request);
        var content = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.FirstName.Should().Be(request.FirstName);
        content.LastName.Should().Be(request.LastName);

        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Theory]
    [InlineData("", "LastName")]
    [InlineData("FirstName", "")]
    [InlineData("", "")]
    public async Task ShouldReturnBadRequest_WhenInvalidInput(string firstName, string lastName)
    {
        // Arrange
        var faker = new Faker();
        var password = faker.StrongPassword();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var request = new UpdateUserRequest(firstName, lastName);

        // Act
        var response = await _client.PutAsJsonAsync("users", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        if (string.IsNullOrEmpty(firstName))
        {
            expected.Errors.Add(nameof(UpdateUserRequest.FirstName), ["'First Name' must not be empty."]);
        }

        if (string.IsNullOrEmpty(lastName))
        {
            expected.Errors.Add(nameof(UpdateUserRequest.LastName), ["'Last Name' must not be empty."]);
        }

        problemDetails.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task Update_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var faker = new Faker();
        var request = new UpdateUserRequest(
            FirstName: faker.Person.FirstName,
            LastName: faker.Person.LastName);

        // Act
        var response = await _client.PutAsJsonAsync("users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
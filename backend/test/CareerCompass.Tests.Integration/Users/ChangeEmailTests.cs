using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Tests.Fakers;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration.Users;

[Collection(nameof(ApiCollection))]
public class ChangeEmailTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    public ChangeEmailTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldChangeEmail_WhenInputIsValid()
    {
        // Arrange
        var password = _faker.StrongPassword();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var request = new ChangeEmailRequest(
            OldPassword: password,
            Email: _faker.Internet.Email());

        // Act
        var response = await _client.PostAsJsonAsync("users/change-email", request);
        var content = await response.Content.ReadFromJsonAsync<ChangeEmailResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Message.Should()
            .Be("Email has been changed successfully. Please login with your new email after confirming it");

        // Cleanup
        await _factory.RemoveUser(user.Id);
    }


    [Fact]
    public async Task ShouldReturnInvalidCredentials_WhenPasswordIsNotCorrect()
    {
        // Arrange
        var password = _faker.StrongPassword();
        var user = await _factory.CreateUser(password);
        var loginRequest = new LoginRequest(user.Email, password);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var request = new ChangeEmailRequest(
            OldPassword: UserFaker.GenerateDifferentPassword(password),
            Email: _faker.Internet.Email());

        // Act
        var response = await _client.PostAsJsonAsync("users/change-email", request);
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var expected = new ProblemDetails
        {
            Title = "Change email: Invalid credentials",
            Status = (int)HttpStatusCode.Conflict,
            Detail = "Invalid credentials provided. Please check your again"
        };
        expected.Extensions.Add("code", "10.30.90.20");
        expected.Extensions.Add("userId", user.Id.ToString());

        content.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenInvalidInput()
    {
        // Arrange
        var userPassword = _faker.StrongPassword();
        var user = await _factory.CreateUser(userPassword);
        var loginRequest = new LoginRequest(user.Email, userPassword);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var request = new ChangeEmailRequest("", "");

        // Act
        var response = await _client.PostAsJsonAsync("users/change-email", request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var expected = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred."
        };


        expected.Errors.Add(nameof(ChangeEmailRequest.OldPassword), ["'Old Password' must not be empty."]);
        expected.Errors.Add("NewEmail",
            ["'New Email' must not be empty.", "'New Email' is not a valid email address."]);

        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var request = new ChangeEmailRequest(
            OldPassword: _faker.StrongPassword(),
            Email: _faker.Internet.Email());

        // Act
        var response = await _client.PostAsJsonAsync("users/change-email", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
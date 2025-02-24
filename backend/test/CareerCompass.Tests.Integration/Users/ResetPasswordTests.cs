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
public class ResetPasswordTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    public ResetPasswordTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldResetPassword_WhenInputIsValid()
    {
        // Arrange
        var oldPassword = _faker.StrongPassword();
        var user = await _factory.CreateUser(oldPassword);
        var loginRequest = new LoginRequest(user.Email, oldPassword);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var newPassword = _faker.StrongPassword();
        var request = new ResetPasswordRequest(
            OldPassword: oldPassword,
            NewPassword: newPassword,
            ConfirmNewPassword: newPassword);

        // Act
        var response = await _client.PostAsJsonAsync("users/reset-password", request);
        var content = await response.Content.ReadFromJsonAsync<ResetPasswordResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content!.Message.Should().Be("Password has been reset successfully. Please login with your new password");


        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Fact]
    public async Task ShouldReturnInvalidCredentials_WhenOldPasswordIsNotCorrect()
    {
        // Arrange
        var oldPassword = _faker.StrongPassword();
        var user = await _factory.CreateUser(oldPassword);
        var loginRequest = new LoginRequest(user.Email, oldPassword);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var newPassword = _faker.StrongPassword();
        var request = new ResetPasswordRequest(
            OldPassword: UserFaker.GenerateDifferentPassword(oldPassword),
            NewPassword: newPassword,
            ConfirmNewPassword: newPassword);

        // Act
        var response = await _client.PostAsJsonAsync("users/reset-password", request);
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var expected = new ProblemDetails
        {
            Title = "Reset password: Invalid credentials",
            Status = (int)HttpStatusCode.Conflict,
            Detail = "Invalid credentials provided. Please check your again"
        };
        expected.Extensions.Add("code", "10.30.80.10");
        expected.Extensions.Add("userId", user.Id.ToString());


        content.ShouldBeEquivalentTo(expected);


        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Fact]
    public async Task ShouldReturnValidationProblemDetails_WhenInvalidEmpty()
    {
        // Arrange
        var userPassword = _faker.StrongPassword();
        var user = await _factory.CreateUser(userPassword);
        var loginRequest = new LoginRequest(user.Email, userPassword);
        var loginResponse = await _client.PostAsJsonAsync("users/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var request = new ResetPasswordRequest("", "", "");

        // Act
        var response = await _client.PostAsJsonAsync("users/reset-password", request);
        var content = await response.Content.ReadAsStringAsync();

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var expected = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        expected.Errors.Add(nameof(ResetPasswordRequest.OldPassword), ["'Old Password' must not be empty."]);
        expected.Errors.Add(nameof(ResetPasswordRequest.NewPassword), ["'New Password' must not be empty."]);
        problemDetails.ShouldBeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var request = new ResetPasswordRequest(
            OldPassword: _faker.StrongPassword(),
            NewPassword: _faker.StrongPassword(),
            ConfirmNewPassword: _faker.StrongPassword());

        // Act
        var response = await _client.PostAsJsonAsync("users/reset-password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
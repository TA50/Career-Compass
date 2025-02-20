using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Tests.Fakers.Api;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Tests.Integration.Users;

[Collection(nameof(ApiCollection))]
public class LoginTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly ICryptoService _cryptoService;


    private readonly Faker<LoginRequest> _loginRequestFaker = new LoginRequestFaker();

    public LoginTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _cryptoService = factory.Services.GetRequiredService<ICryptoService>();
    }

    [Fact]
    public async Task Return200Success_WhenRequestIsValid()
    {
        // Arrange
        var request = await CreateUser();


        // Act
        var response = await _client.PostAsJsonAsync("/users/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Cleanup
        await RemoveUser(request.Email);
    }

    [Fact]
    public async Task Return409Conflict_WhenPasswordIsNotCorrect()
    {
        // Arrange
        var validLoginRequest = await CreateUser();
        var differentPassword = UserFaker.GenerateDifferentPassword(validLoginRequest.Password);

        var request = _loginRequestFaker
            .Clone()
            .RuleFor(r => r.Email, f => validLoginRequest.Email)
            .RuleFor(r => r.Password, f => differentPassword)
            .Generate();

        // Act
        var response = await _client.PostAsJsonAsync("/users/login", request);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        body.Should().NotBeNull();
        body.Title.Should().Be("Invalid credentials");
        body.Status.Should().Be(StatusCodes.Status409Conflict);
        var errorCode = body.Extensions["code"]?.ToString() ?? string.Empty;
        errorCode.Should().Be("10.30.60.10");
        var email = body.Extensions["email"]?.ToString() ?? string.Empty;
        email.Should().Be(request.Email);


        // Cleanup
        await RemoveUser(validLoginRequest.Email);
    }

    [Fact]
    public async Task Return409Conflict_WhenEmailIsNotCorrect()
    {
        // Arrange
        var validLoginRequest = await CreateUser();
        var differentEmail = UserFaker.GenerateDifferentEmail(validLoginRequest.Email);
        var request = _loginRequestFaker
            .Clone()
            .RuleFor(r => r.Email, f => differentEmail)
            .RuleFor(r => r.Password, f => validLoginRequest.Password)
            .Generate();

        // Act
        var response = await _client.PostAsJsonAsync("/users/login", request);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        body.Should().NotBeNull();
        body.Title.Should().Be("Invalid credentials");
        body.Status.Should().Be(StatusCodes.Status409Conflict);
        var errorCode = body.Extensions["code"]?.ToString() ?? string.Empty;
        errorCode.Should().Be("10.30.60.10");
        var email = body.Extensions["email"]?.ToString() ?? string.Empty;
        email.Should().Be(request.Email);


        // Cleanup
        await RemoveUser(validLoginRequest.Email);
    }

    [Fact]
    public async Task Return400_WhenEmailIsEmpty()
    {
        // Arrange
        var request = _loginRequestFaker
            .Clone()
            .RuleFor(r => r.Email, f => string.Empty)
            .Generate();


        // Act
        var response = await _client.PostAsJsonAsync("/users/login", request);
        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().NotBeNull();
        body.Title.Should().Be("One or more validation errors occurred.");
        body.Status.Should().Be(StatusCodes.Status400BadRequest);
        body.Errors.Should().ContainKey(nameof(LoginRequest.Email)).WhoseValue.Should()
            .Contain("'Email' is not a valid email address.");
    }

    [Fact]
    public async Task Return400_WhenEmailIsInvalid()
    {
        // Arrange
        var request = _loginRequestFaker
            .Clone()
            .RuleFor(r => r.Email, f => "1231")
            .Generate();


        // Act
        var response = await _client.PostAsJsonAsync("/users/login", request);
        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().NotBeNull();
        body.Title.Should().Be("One or more validation errors occurred.");
        body.Status.Should().Be(StatusCodes.Status400BadRequest);
        body.Errors.Should().ContainKey(nameof(LoginRequest.Email)).WhoseValue.Should()
            .Contain("'Email' is not a valid email address.");
    }

    [Fact]
    public async Task Return400_WhenPasswordIsEmpty()
    {
        // Arrange
        var request = _loginRequestFaker
            .Clone()
            .RuleFor(r => r.Password, f => string.Empty)
            .Generate();


        // Act
        var response = await _client.PostAsJsonAsync("/users/login", request);
        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().NotBeNull();
        body.Title.Should().Be("One or more validation errors occurred.");
        body.Status.Should().Be(StatusCodes.Status400BadRequest);
        body.Errors.Should().ContainKey(nameof(LoginRequest.Password)).WhoseValue.Should()
            .Contain("'Password' must not be empty.");
    }


    private async Task<LoginRequest> CreateUser()
    {
        var coreSettings = _factory.Services.GetRequiredService<CoreSettings>();
        var user = new UserFaker()
            .Generate();
        var password = user.Password;
        var hash = _cryptoService.Hash(password);
        user.SetPassword(hash);
        var code = user.GenerateEmailConfirmationCode(
            TimeSpan.FromHours(coreSettings.EmailConfirmationCodeLifetimeInHours));
        user.ConfirmEmail(code);

        _factory.DbContext.Users.Add(user);
        await _factory.DbContext.SaveChangesAsync();

        return _loginRequestFaker
            .Clone()
            .RuleFor(r => r.Email, f => user.Email)
            .RuleFor(r => r.Password, f => password)
            .Generate();
    }

    private async Task RemoveUser(string email)
    {
        await _factory.DbContext.Users.Where(u => u.Email == email).ExecuteDeleteAsync();
    }
}
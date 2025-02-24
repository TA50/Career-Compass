using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Users;
using CareerCompass.Infrastructure.Persistence;
using CareerCompass.Tests.Fakers;
using CareerCompass.Tests.Fakers.Core;
using CareerCompass.Tests.Integration.EmailServerClient;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Tests.Integration.Users;

[Collection(nameof(ApiCollection))]
public class ForgotPasswordTests
{
    private readonly HttpClient _client;
    private readonly AppDbContext _dbContext;
    private readonly IEmailServerClient _emailServerClient;
    private readonly UserFaker _userFaker = new();
    private readonly CareerCompassApiFactory _factory;
    private readonly CoreSettings _coreSettings;
    private readonly ICryptoService _cryptoService;

    public ForgotPasswordTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _emailServerClient = factory.EmailServerClient;
        _dbContext = factory.DbContext;
        _coreSettings = _factory.Services.GetRequiredService<CoreSettings>();
        _cryptoService = _factory.Services.GetRequiredService<ICryptoService>();
    }

    #region Generate Frogot Password Code

    [Fact]
    public async Task SendForgotPasswordEmailWithCode_WhenInputIsValid()
    {
        // Arrange
        var user = await _factory.CreateUser();

        // Act
        var result = await _client.GetAsync($"/users/forgot-password?email={user.Email}");
        // Assert

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var forgotPasswordCode = await _dbContext.Users.Where(u => u.Id == user.Id)
            .Select(u => u.ForgotPasswordCode)
            .FirstAsync();

        var messages = await _emailServerClient.GetMessages();
        var userMessages = messages.FirstOrDefault(m => m.To.Contains(user.Email));

        userMessages.Should().NotBeNull();
        var rawMessage = await _emailServerClient.GetRawMessage(userMessages.Id);

        userMessages.Subject.Should().Be("Change your password");
        forgotPasswordCode.Should().NotBeNull();
        rawMessage.Should().Contain(forgotPasswordCode);


        // Cleanup
        await _factory.RemoveUser(user.Email);
    }


    [Fact]
    public async Task ShouldReturnValidationProblem_WhenEmailDoesNotBelongToUser()
    {
        // Arrange
        var email = _userFaker.Generate().Email;

        // Act
        var result = await _client.GetAsync($"/users/forgot-password?email={email}");

        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();
        // Assert

        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Forgot password: invalid credentials");
        problemDetails.Detail.Should().Be($"Provided email ({email}) does not exist");
        var emailExtension = problemDetails.Extensions["email"]?.ToString() ?? string.Empty;
        emailExtension.Should()
            .NotBeNull()
            .And.Contain(email);

        var code = problemDetails.Extensions["code"]?.ToString() ?? string.Empty;
        code.Should().NotBeNull()
            .And.Be("10.30.70.20");
    }

    [Fact]
    public async Task ShouldReturnValidationProblem_WhenEmailIsInvalid()
    {
        var email = "invalid-email";
        // Act
        var result = await _client.GetAsync($"/users/forgot-password?email={email}");
        var problem = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var expectedProblem = new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            {
                "Email", [
                    "'Email' is not a valid email address."
                ]
            }
        })
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };


        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problem.Should().BeEquivalentTo(expectedProblem);
    }

    #endregion

    #region Change Forgotten Password

    [Fact]
    public async Task ShouldChangePassword_WhenInputIsValid()
    {
        // Arrange
        var user = await _factory.CreateUser();
        user = await _dbContext.Users.FirstAsync(u => u.Id == user.Id);

        var forgotPasswordCode =
            user.GenerateForgotPasswordCode(TimeSpan.FromHours(_coreSettings.ForgotPasswordCodeLifetimeInHours));

        await _dbContext.SaveChangesAsync();
        var password = new Faker().StrongPassword();

        var request = new ChangeForgottenPasswordRequest(user.Email, forgotPasswordCode, password, password);

        // Act
        var response = await _client.PostAsJsonAsync("/users/change-forgotten-password", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedUser = await _dbContext.Users
            .AsNoTracking() // Force EF to get the data from the database and not from the cache
            .FirstAsync(u => u.Id == user.Id);
        var verificationResult = _cryptoService.Verify(password, updatedUser.Password);
        verificationResult.Should().BeTrue();

        // Cleanup

        await _factory.RemoveUser(user.Email);
    }

    [Fact]
    public async Task ShouldReturnInvalidCode_WhenCodeIsNotCorrect()
    {
        // Arrange
        var user = await _factory.CreateUser();
        user = await _dbContext.Users.FirstAsync(u => u.Id == user.Id);

        var forgotPasswordCode =
            user.GenerateForgotPasswordCode(TimeSpan.FromHours(_coreSettings.ForgotPasswordCodeLifetimeInHours));


        await _dbContext.SaveChangesAsync();
        var password = new Faker().StrongPassword();
        var code = UserFaker.GenerateDifferentCode(forgotPasswordCode);

        var request = new ChangeForgottenPasswordRequest(user.Email, code, password, password);

        // Act
        var response = await _client.PostAsJsonAsync("/users/change-forgotten-password", request);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        problem.Should().NotBeNull();
        problem.Title.Should().Be("Change Forgotten password: Invalid code");
        problem.Detail.Should().Be($"Invalid code provided for email ({user.Email})");
        problem.Status.Should().Be(StatusCodes.Status409Conflict);
        var codeExtension = problem.Extensions["code"]?.ToString() ?? string.Empty;
        codeExtension.Should().NotBeNull()
            .And.Be("10.30.70.30");

        var emailExtension = problem.Extensions["email"]?.ToString() ?? string.Empty;
        emailExtension.Should().NotBeNull()
            .And.Be(user.Email);

        // Cleanup

        await _factory.RemoveUser(user.Email);
    }

    [Fact]
    public async Task ShouldReturnInvalidCode_WhenCodeIsExpired()
    {
        // Arrange
        var user = await _factory.CreateUser();
        user = await _dbContext.Users.FirstAsync(u => u.Id == user.Id);

        var forgotPasswordCode =
            user.GenerateForgotPasswordCode(TimeSpan.FromHours(-1));


        await _dbContext.SaveChangesAsync();
        var password = new Faker().StrongPassword();
        var code = UserFaker.GenerateDifferentCode(forgotPasswordCode);

        var request = new ChangeForgottenPasswordRequest(user.Email, code, password, password);

        // Act
        var response = await _client.PostAsJsonAsync("/users/change-forgotten-password", request);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        problem.Should().NotBeNull();
        problem.Title.Should().Be("Change Forgotten password: Invalid code");
        problem.Detail.Should().Be($"Invalid code provided for email ({user.Email})");
        problem.Status.Should().Be(StatusCodes.Status409Conflict);
        var codeExtension = problem.Extensions["code"]?.ToString() ?? string.Empty;
        codeExtension.Should().NotBeNull()
            .And.Be("10.30.70.30");

        var emailExtension = problem.Extensions["email"]?.ToString() ?? string.Empty;
        emailExtension.Should().NotBeNull()
            .And.Be(user.Email);

        // Cleanup

        await _factory.RemoveUser(user.Email);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenEmailIsNotCorrect()
    {
        // Arrange
        var user = await _factory.CreateUser();
        user = await _dbContext.Users.FirstAsync(u => u.Id == user.Id);

        var forgotPasswordCode =
            user.GenerateForgotPasswordCode(TimeSpan.FromHours(_coreSettings.ForgotPasswordCodeLifetimeInHours));


        await _dbContext.SaveChangesAsync();
        var password = new Faker().StrongPassword();
        var differentEmail = UserFaker.GenerateDifferentEmail(user.Email);
        var request = new ChangeForgottenPasswordRequest(differentEmail, forgotPasswordCode, password, password);

        // Act
        var response = await _client.PostAsJsonAsync("/users/change-forgotten-password", request);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        problem.Should().NotBeNull();
        problem.Status.Should().Be(StatusCodes.Status409Conflict);
        problem.Title.Should().Be("Change Forgotten password: invalid email");
        problem.Detail.Should().Be($"Provided email ({request.Email}) does not exist");
        var codeExtension = problem.Extensions["code"]?.ToString() ?? string.Empty;
        codeExtension.Should().NotBeNull()
            .And.Be("10.30.70.20");

        var emailExtension = problem.Extensions["email"]?.ToString() ?? string.Empty;
        emailExtension.Should().NotBeNull()
            .And.Be(request.Email);

        // Cleanup

        await _factory.RemoveUser(user.Email);
    }

    [Fact]
    public async Task ShouldReturnValidationProblemDetails_WhenFieldsAreEmpty()
    {
        // Arrange
        var user = await _factory.CreateUser();
        user = await _dbContext.Users.FirstAsync(u => u.Id == user.Id);

        var forgotPasswordCode =
            user.GenerateForgotPasswordCode(TimeSpan.FromHours(_coreSettings.ForgotPasswordCodeLifetimeInHours));


        await _dbContext.SaveChangesAsync();
        var request = new ChangeForgottenPasswordRequest("", "", "", "");

        // Act
        var response = await _client.PostAsJsonAsync("/users/change-forgotten-password", request);
        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().NotBeNull();
        body.Title.Should().Be("One or more validation errors occurred.");
        body.Status.Should().Be(StatusCodes.Status400BadRequest);

        body.Errors.Should().ContainKey(nameof(ChangeForgottenPasswordRequest.Email)).WhoseValue.Should()
            .Contain("'Email' must not be empty.")
            .And
            .Contain("'Email' is not a valid email address.");

        body.Errors.Should()
            .ContainKey(nameof(ChangeForgottenPasswordRequest.Code))
            .WhoseValue.Should()
            .Contain("'Code' must not be empty.");

        body.Errors.Should().ContainKey(nameof(ChangeForgottenPasswordRequest.NewPassword)).WhoseValue.Should()
            .Contain("'New Password' must not be empty.")
            .And.Contain("'New Password' is not in the correct format.");


        // Cleanup

        await _factory.RemoveUser(user.Email);
    }

    #endregion
}
using System.Net;
using System.Net.Http.Json;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Tests.Fakers.Api;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tests.Integration.Users;

[Collection(nameof(ApiCollection))]
public class RegistrationTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private RegisterRequestFaker _registerRequestFaker = new();

    public RegistrationTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task RegisterNewUser_WhenInputIsValid()
    {
        // Arrange
        var request = _registerRequestFaker.Generate();
        // Act

        var response = await _client.PostAsJsonAsync("/users", request);
        var body = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        if (body is null)
        {
            throw new AssertionFailedException("Response body is null");
        }

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body.UserId.Should().NotBeEmpty();
        body.Message.Should().Be("User has been registered successfully. Please confirm your email");
        var user = await _factory.DbContext.Users
            .AsNoTracking()
            .FirstAsync(u => u.Email == request.Email);

        user.Should().NotBeNull();

        user.EmailConfirmationCode.Should().NotBeNull();
        var messages = await _factory.EmailServerClient.GetMessages();
        var userConfirmationEmail = messages.First(m => m.To.Contains(request.Email));
        userConfirmationEmail.Should().NotBeNull();
        userConfirmationEmail.Subject.Should().Be("Welcome to Career Compass");
        var rawMessage = await _factory.EmailServerClient.GetRawMessage(userConfirmationEmail.Id);
        rawMessage.Should().Contain(user.EmailConfirmationCode);

        // Cleanup

        await _factory.DbContext.Users.Where(u => u.Id == user.Id).ExecuteDeleteAsync();
    }

    [Fact]
    public async Task Return409Conflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var user = new UserFaker().Generate();
        await _factory.DbContext.Users.AddAsync(user);
        await _factory.DbContext.SaveChangesAsync();

        var request = _registerRequestFaker.Generate();
        request.Email = user.Email;

        // Act
        var response = await _client.PostAsJsonAsync("/users", request);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        body.Should().NotBeNull();

        body.Title.Should().Be("User with email already exists");
        body.Status.Should().Be(StatusCodes.Status409Conflict);
        body.Detail.Should().Be($"User with email ({user.Email}) already exists");
        var email = body.Extensions["email"]?.ToString() ?? "not found";
        var code = body.Extensions["code"]?.ToString() ?? "not found";
        
        email.Should().Be(user.Email);
        code.Should().Be("10.30.10.10");
        


        // Cleanup
        await _factory.DbContext.Users.Where(u => u.Id == user.Id).ExecuteDeleteAsync();
    }


    [Theory]
    [MemberData(nameof(GetInvalidRequests))]
    public async Task Return400BadRequest_WhenRequestIsInvalid(InvalidRegisterRequestData invalidRequest)
    {
        // Arrange

        var request = invalidRequest.Request;
        // Act
        var response = await _client.PostAsJsonAsync("/users", request);
        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().NotBeNull();
        body.Title.Should().Be("One or more validation errors occurred.");
        body.Status.Should().Be(StatusCodes.Status400BadRequest);
        foreach (var error in invalidRequest.ErrorMessages)
        {
            body.Errors.Should().ContainKey(invalidRequest.PropertyName).WhoseValue.Should().Contain(error);
        }
    }


    public static IEnumerable<object[]> GetInvalidRequests()
    {
        var faker = new RegisterRequestFaker();
        var emptyEmail = faker.Generate();
        emptyEmail.Email = string.Empty;
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = emptyEmail,
                PropertyName = nameof(RegisterRequest.Email),
                ErrorMessages =
                [
                    "'Email' must not be empty.",
                    "'Email' is not a valid email address."
                ]
            }
        ];

        var invalidEmail = faker.Generate();
        invalidEmail.Email = "invalid-email";
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = invalidEmail,
                PropertyName = nameof(RegisterRequest.Email),
                ErrorMessages = new[] { "'Email' is not a valid email address." }
            }
        ];

        var emptyFirstName = faker.Generate();
        emptyFirstName.FirstName = string.Empty;
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = emptyFirstName,
                PropertyName = nameof(RegisterRequest.FirstName),
                ErrorMessages = ["'First Name' must not be empty."]
            }
        ];

        var emptyLastname = faker.Generate();
        emptyLastname.LastName = string.Empty;
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = emptyLastname,
                PropertyName = nameof(RegisterRequest.LastName),
                ErrorMessages = ["'Last Name' must not be empty."]
            }
        ];

        var emptyPassword = faker.Generate();
        emptyPassword.Password = string.Empty;
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = emptyPassword,
                PropertyName = nameof(RegisterRequest.Password),
                ErrorMessages =
                [
                    "'Password' is not in the correct format.",
                    "'Password' must not be empty."
                ]
            }
        ];

        var invalidPassword = faker.Generate();
        invalidPassword.Password = string.Empty;
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = invalidPassword,
                PropertyName = nameof(RegisterRequest.Password),
                ErrorMessages =
                [
                    "'Password' is not in the correct format.",
                ]
            }
        ];

        var emptyConfirmPassword = faker.Generate();
        emptyConfirmPassword.ConfirmPassword = string.Empty;
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = emptyConfirmPassword,
                PropertyName = nameof(RegisterRequest.ConfirmPassword),
                ErrorMessages =

                [
                    "'Confirm Password' must not be empty.",
                    $"'Confirm Password' must be equal to '{emptyConfirmPassword.Password}'."
                ]
            }
        ];
        var invalidConfirmPassword = faker.Generate();
        invalidConfirmPassword.ConfirmPassword = "invalid-password";
        yield return
        [
            new InvalidRegisterRequestData
            {
                Request = invalidConfirmPassword,
                PropertyName = nameof(RegisterRequest.ConfirmPassword),
                ErrorMessages =

                [
                    $"'Confirm Password' must be equal to '{invalidConfirmPassword.Password}'."
                ]
            }
        ];
    }
}

public class InvalidRegisterRequestData
{
    public RegisterRequest Request { get; set; }
    public string PropertyName { get; set; }
    public string[] ErrorMessages { get; set; }
}
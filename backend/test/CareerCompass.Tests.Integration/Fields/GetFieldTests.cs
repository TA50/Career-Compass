using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Fields;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CareerCompass.Tests.Integration.Fields;

[Collection(nameof(ApiCollection))]
public class GetFieldTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;
    private const int ValidNameLength = 10;

    public GetFieldTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldGetAllUserFields_WhenAuthenticated()
    {
        // Arrange
        var userId = await Login();
        var fields = new List<Field>
        {
            Field.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength),
                _faker.Random.AlphaNumeric(ValidNameLength)),
            Field.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength),
                _faker.Random.AlphaNumeric(ValidNameLength)),
            Field.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength),
                _faker.Random.AlphaNumeric(ValidNameLength))
        };
        _factory.DbContext.Fields.AddRange(fields);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("fields");
        var content = await response.Content.ReadFromJsonAsync<List<FieldDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().HaveCount(3);
        var expected = fields.Select(x => new FieldDto(x.Id.ToString(), x.Name, x.Group)).ToList();
        content.Should().BeEquivalentTo(expected);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldGetSpecificField_WhenFieldExists()
    {
        // Arrange
        var userId = await Login();
        var field = Field.Create(userId, _faker.Random.AlphaNumeric(ValidNameLength),
            _faker.Random.AlphaNumeric(ValidNameLength));
        _factory.DbContext.Fields.Add(field);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"fields/{field.Id}");
        var content = await response.Content.ReadFromJsonAsync<FieldDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Id.Should().Be(field.Id.ToString());
        content.Name.Should().Be(field.Name);
        content.Group.Should().Be(field.Group);

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFieldDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var nonExistentFieldId = FieldId.CreateUnique();

        // Act
        var response = await _client.GetAsync($"fields/{nonExistentFieldId}");
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("FieldRead: Field with the given user id and field id was not found!");
        problemDetails.Detail.Should().Contain(userId.ToString());
        problemDetails.Detail.Should().Contain(nonExistentFieldId.ToString());

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenUserHasNoFields()
    {
        // Arrange
        var userId = await Login();

        // Act
        var response = await _client.GetAsync("fields");
        var content = await response.Content.ReadFromJsonAsync<List<FieldDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().BeEmpty();

        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("fields");

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
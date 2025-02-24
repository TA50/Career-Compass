using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCompass.Tests.Integration.Fields;

[Collection(nameof(ApiCollection))]
public class DeleteFieldTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;

    public DeleteFieldTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
    }

    [Fact]
    public async Task ShouldDeleteField_WhenFieldExists()
    {
        // Arrange
        var userId = await Login();
        var field = Field.Create(userId, _faker.Random.AlphaNumeric(10), _faker.Random.AlphaNumeric(10));
        _factory.DbContext.Fields.Add(field);
        await _factory.DbContext.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"fields/{field.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var entity = await _factory.DbContext.Fields.FirstOrDefaultAsync(f => f.Id == field.Id);
        entity.Should().BeNull();

        // Cleanup

        await _factory.RemoveUser(userId);
    }


    [Fact]
    public async Task ShouldReturnNotFound_WhenFieldDoesNotExist()
    {
        // Arrange
        var userId = await Login();
        var nonExistentFieldId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"fields/{nonExistentFieldId}");
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var expected = new ProblemDetails
        {
            Title = "FieldDelete: Field with the given id was not found!",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"Field with id '{nonExistentFieldId}' was not found",
        };
        problemDetails.Should().NotBeNull();


        problemDetails.ShouldBeEquivalentTo(expected);
        // Cleanup
        await _factory.RemoveUser(userId);
    }

    [Fact]
    public async Task ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var fieldId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"fields/{fieldId}");

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
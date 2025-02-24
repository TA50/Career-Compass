using System.Net;
using System.Net.Http.Json;
using Bogus;
using CareerCompass.Api.Contracts.Users;
using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CareerCompass.Tests.Integration.Users;

[Collection(nameof(ApiCollection))]
public class ConfirmEmailTests
{
    private readonly CareerCompassApiFactory _factory;
    private readonly HttpClient _client;
    private readonly Faker _faker;
    private readonly CoreSettings _coreSettings;

    public ConfirmEmailTests(CareerCompassApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _faker = new Faker();
        _coreSettings = _factory.Services.GetRequiredService<CoreSettings>();
    }

    [Fact]
    public async Task ShouldConfirmEmail_WhenCodeIsValid()
    {
        // Arrange
        var user = await CreateUnConfirmedUser();
        // Act
        var response = await _client.GetAsync($"users/confirm-email/{user.Id}/{user.EmailConfirmationCode}");


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedUser = await _factory.DbContext.Users.AsNoTracking()
            .Where(u => u.Id == user.Id)
            .Select(u => u.EmailConfirmed)
            .FirstAsync();
        updatedUser.Should().BeTrue();
        // Cleanup
        await _factory.RemoveUser(user.Id);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenCodeIsInvalid()
    {
        // Arrange
        var user = await CreateUnConfirmedUser();
        var differentCode = UserFaker.GenerateDifferentCode(user.EmailConfirmationCode ?? string.Empty);
        // Act
        var response = await _client.GetAsync($"users/confirm-email/{user.Id}/{differentCode}");
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var expected = new ProblemDetails
        {
            Title = "Email confirmation error, invalid confirmation code",
            Detail =
                $"Email confirmation error for user with userId ({user.Id}). the provided confirmation code is not correct",
            Status = StatusCodes.Status400BadRequest
        };
        expected.Extensions.Add("code", "10.30.50.20");
        expected.Extensions.Add("userId", user.Id.ToString());

        content.Should().NotBeNull();
        content.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenCodeIsExpired()
    {
        // Arrange
        var user = await CreateUnConfirmedUser();
        var code = user.GenerateEmailConfirmationCode(TimeSpan.FromHours(-1));
        var differentCode = UserFaker.GenerateDifferentCode(code);
        // Act
        var response = await _client.GetAsync($"users/confirm-email/{user.Id}/{differentCode}");
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var expected = new ProblemDetails
        {
            Title = "Email confirmation error, invalid confirmation code",
            Detail =
                $"Email confirmation error for user with userId ({user.Id}). the provided confirmation code is not correct",
            Status = StatusCodes.Status400BadRequest
        };
        expected.Extensions.Add("code", "10.30.50.20");
        expected.Extensions.Add("userId", user.Id.ToString());

        content.Should().NotBeNull();
        content.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.Empty;
        // Act
        var response = await _client.GetAsync($"users/confirm-email/{userId}/12121");
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var expected = new ProblemDetails
        {
            Title = "Email confirmation error, user not found",
            Detail = $"Email confirmation error. User with userId ({userId}) was not found",
            Status = StatusCodes.Status400BadRequest
        };
        expected.Extensions.Add("code", "10.30.50.10");
        expected.Extensions.Add("userId", userId.ToString());

        content.Should().NotBeNull();
        content.ShouldBeEquivalentTo(expected);
    }

    private async Task<User> CreateUnConfirmedUser()
    {
        var cryptoService = _factory.Services.GetRequiredService<ICryptoService>();
        var userFaker = new UserFaker();
        var user = userFaker.Generate();
        var hash = cryptoService.Hash(user.Password);
        user.SetPassword(hash);

        user.GenerateEmailConfirmationCode(
            TimeSpan.FromHours(_coreSettings.EmailConfirmationCodeLifetimeInHours));

        _factory.DbContext.Users.Add(user);
        await _factory.DbContext.SaveChangesAsync();
        return user;
    }
}
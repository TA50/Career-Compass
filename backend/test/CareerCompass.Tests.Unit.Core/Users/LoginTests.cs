using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.Login;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class LoginTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ICryptoService _cryptoService = Substitute.For<ICryptoService>();
    private readonly UserFaker _userFaker = new();

    private readonly ILoggerAdapter<LoginCommandHandler>
        _logger = Substitute.For<ILoggerAdapter<LoginCommandHandler>>();

    private readonly LoginCommandHandler _sut;

    public LoginTests()
    {
        _sut = new LoginCommandHandler(_userRepository, _cryptoService, _logger);
    }

    [Fact]
    public async Task ShouldReturnUserId_WhenCredentialsAreValid()
    {
        // Arrange
        var user = _userFaker.Generate();
        var request = new LoginCommand(user.Email, user.Password);
        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();

        var code = user.GenerateEmailConfirmationCode();
        user.ConfirmEmail(code);

        _cryptoService.Verify(request.Password, user.Password).Returns(true);
        _userRepository
            .Single(spec, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        _logger.Received(1).LogInformation("Trying to login user with email: {Email}", request.Email);
        _logger.Received(1).LogInformation("Login successful for user {Email}", request.Email);

        result.IsError.Should().BeFalse();
        result.Value.UserId.Should().NotBeNull();
        result.Value.UserId.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnInvalidCredentials_WhenEmailIsWrong()
    {
        // Arrange
        var user = _userFaker.Generate();
        var request = new LoginCommand("invalid@email", user.Password);

        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();

        var code = user.GenerateEmailConfirmationCode();
        user.ConfirmEmail(code);

        _userRepository
            .Single(spec, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        _logger.Received(1).LogInformation("Trying to login user with email: {Email}", request.Email);
        _logger.Received(1).LogInformation("user not found with this email: {Email}", request.Email);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.UserLogin_InvalidCredentials(request.Email));
    }

    [Fact]
    public async Task ShouldReturnInvalidCredentials_WhenPasswordIsWrong()
    {
        // Arrange
        var user = _userFaker.Generate();
        var wrongPassword = UserFaker.GenerateDifferentPassword(user.Password);
        var request = new LoginCommand(user.Email, wrongPassword);

        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();

        var code = user.GenerateEmailConfirmationCode();
        user.ConfirmEmail(code);

        _cryptoService.Verify(request.Password, user.Password).Returns(false);
        _userRepository
            .Single(spec, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Trying to login user with email: {Email}", request.Email);
        _logger.Received(1).LogInformation("provided wrong password for user with email: {Email}", request.Email);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.UserLogin_InvalidCredentials(request.Email));
    }

    [Fact]
    public async Task ShouldReturnInvalidCredentials_WhenEmailIsNotConfirmed()
    {
        // Arrange
        var user = _userFaker.Generate();
        var request = new LoginCommand(user.Email, user.Password);
        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();

        _cryptoService.Verify(request.Password, user.Password).Returns(true);
        _userRepository
            .Single(spec, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Trying to login user with email: {Email}", request.Email);
        _logger.Received(1).LogInformation("user not found with this email: {Email}", request.Email);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.UserLogin_InvalidCredentials(request.Email));
    }
}
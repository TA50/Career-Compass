using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.ChangeEmail;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class ChangeEmailTests
{
    private readonly UserFaker _userFaker = new();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ICryptoService _cryptoService = Substitute.For<ICryptoService>();

    private readonly ILoggerAdapter<ChangeEmailCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<ChangeEmailCommandHandler>>();

    private readonly ChangeEmailCommandHandler _sut;

    public ChangeEmailTests()
    {
        _sut = new(_userRepository, _cryptoService, _logger);
    }

    [Fact]
    public async Task ShouldReturnUserIdAndCode_WhenInputIsValid()
    {
        // Arrange
        var user = _userFaker.Generate();
        var newMail = new Faker().Internet.Email();
        var request = new ChangeEmailCommand(user.Id, user.Password, newMail);

        _cryptoService.Verify(request.OldPassword, user.Password).Returns(true);
        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult());

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.UserId.Should().Be(user.Id);
        result.Value.EmailConfirmationCode.Should().NotBeNullOrEmpty();
        user.EmailConfirmationCode.Should().NotBeNullOrEmpty();
        result.Value.EmailConfirmationCode.Should().Be(user.EmailConfirmationCode);

        _logger.Received(1).LogInformation("Updating email for user with id {UserId}", user.Id);
        _logger.Received(1).LogInformation("Email updated for user with id {UserId}", user.Id);
    }

    [Fact]
    public async Task ShouldReturnEmailAlreadyExists_WhenNewEmailHasBeenUsed()
    {
        // Arrange
        var user = _userFaker.Generate();
        var newMail = new Faker().Internet.Email();
        var request = new ChangeEmailCommand(user.Id, user.Password, newMail);

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        _cryptoService.Verify(request.OldPassword, user.Password).Returns(true);
        var spec = new GetUserByEmailSpecification(newMail)
            .ExcludeUser(user.Id);

        _userRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(true);
        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult());
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ChangeEmail_EmailAlreadyExists(request.NewEmail));

        _logger.Received(1).LogInformation("Updating email for user with id {UserId}", user.Id);
    }


    [Fact]
    public async Task ShouldReturnInvalidCredentials_WhenPasswordIsWrong()
    {
        var user = _userFaker.Generate();
        var newMail = new Faker().Internet.Email();
        var request = new ChangeEmailCommand(user.Id, user.Password, newMail);

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        _cryptoService.Verify(request.OldPassword, user.Password).Returns(false);
        var spec = new GetUserByEmailSpecification(newMail)
            .ExcludeUser(user.Id);

        _userRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult());
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ChangeEmail_InvalidCredentials(request.UserId));

        _logger.Received(1).LogInformation("Updating email for user with id {UserId}", user.Id);
    }

    [Fact]
    public async Task ShouldReturnUserNotFound_WhenUserWasNotFound()
    {
        var user = _userFaker.Generate();
        var newMail = new Faker().Internet.Email();
        var request = new ChangeEmailCommand(user.Id, user.Password, newMail);

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).ReturnsNull();
        _cryptoService.Verify(request.OldPassword, user.Password).Returns(true);
        var spec = new GetUserByEmailSpecification(newMail)
            .ExcludeUser(user.Id);

        _userRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult());
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ChangeEmail_UserNotFound(request.UserId));

        _logger.Received(1).LogInformation("Updating email for user with id {UserId}", user.Id);
    }

    [Fact]
    public async Task ShouldReturnOperationFailedError_WhenDbFails()
    {
        var user = _userFaker.Generate();
        var newMail = new Faker().Internet.Email();
        var request = new ChangeEmailCommand(user.Id, user.Password, newMail);
        const string errorMessage = "Database error";
        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        _cryptoService.Verify(request.OldPassword, user.Password).Returns(true);
        var spec = new GetUserByEmailSpecification(newMail)
            .ExcludeUser(user.Id);

        _userRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult(errorMessage));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ChangeEmail_OperationFailed(request.UserId));

        _logger.Received(1).LogInformation("Updating email for user with id {UserId}", user.Id);
        _logger.Received(1).LogError("Failed to update email for user with id {UserId}: {ErrorMessage}", user.Id,
            errorMessage);
    }
}
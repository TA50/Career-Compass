using Bogus;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.ResetPassword;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class ResetPasswordTests
{
    private readonly UserFaker _userFaker = new();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<ResetPasswordCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<ResetPasswordCommandHandler>>();

    private readonly ICryptoService _cryptoService = Substitute.For<ICryptoService>();
    private readonly ResetPasswordCommandHandler _sut;

    public ResetPasswordTests()
    {
        _sut = new ResetPasswordCommandHandler(_userRepository, _cryptoService, _logger);
    }

    [Fact]
    public async Task ShouldResetPassword_WhenInputIsValid()
    {
        // Arrange
        var user = _userFaker.Generate();
        var newPassword = UserFaker.GenerateDifferentPassword(user.Password);
        var request = new ResetPasswordCommand(user.Id, user.Password, newPassword, newPassword);

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        _cryptoService.Verify(request.OldPassword, user.Password).Returns(true);
        _cryptoService.Hash(request.NewPassword).Returns(request.NewPassword);

        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult());

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.UserId.Should().Be(user.Id);
        user.Password.Should().Be(request.NewPassword);

        _logger.Received(1).LogInformation("Resetting password for user with id {UserId}", user.Id);
        _logger.Received(1).LogInformation("Password reset for user with id {UserId}", user.Id);
    }

    [Fact]
    public async Task ShouldReturnInvalidCredentialsError_WhenUserWasNotFound()
    {
        // Arrange
        var user = _userFaker.Generate();
        var newPassword = UserFaker.GenerateDifferentPassword(user.Password);
        var request = new ResetPasswordCommand(user.Id, user.Password, newPassword, newPassword);

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ResetPassword_InvalidCredentials(user.Id));

        _logger.Received(1).LogInformation("Resetting password for user with id {UserId}", user.Id);
        _logger.Received(1)
            .LogInformation("Failed to reset password for user with id {UserId}: user does not exist", user.Id);
    }

    [Fact]
    public async Task ShouldReturnInvalidCredentialsError_WhenPasswordIsWrong()
    {
        // Arrange
        var user = _userFaker.Generate();
        var newPassword = UserFaker.GenerateDifferentPassword(user.Password);
        var request = new ResetPasswordCommand(user.Id, user.Password, newPassword, newPassword);

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        _cryptoService.Verify(request.OldPassword, user.Password).Returns(false);
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ResetPassword_InvalidCredentials(user.Id));

        _logger.Received(1).LogInformation("Resetting password for user with id {UserId}", user.Id);
        _logger.Received(1)
            .LogInformation("Failed to reset password for user with id {UserId}: old password does not match", user.Id);
    }

    [Fact]
    public async Task ShouldReturnOperationFailedError_WhenDbFails()
    {
        // Arrange
        var user = _userFaker.Generate();
        var newPassword = UserFaker.GenerateDifferentPassword(user.Password);
        var request = new ResetPasswordCommand(user.Id, user.Password, newPassword, newPassword);

        const string dbError = "Database error";

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);


        _cryptoService.Verify(request.OldPassword, user.Password).Returns(true);
        _cryptoService.Hash(request.NewPassword).Returns(request.NewPassword);
        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult(dbError));

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ResetPassword_OperationFailed(user.Id));

        _logger.Received(1).LogInformation("Resetting password for user with id {UserId}", user.Id);
        _logger.Received(1)
            .LogError("Failed to reset password for user with id {UserId}: {ErrorMessage}", request.UserId,
                dbError);
    }
}
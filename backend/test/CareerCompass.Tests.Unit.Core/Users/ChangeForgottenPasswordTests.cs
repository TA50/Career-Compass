using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.ChangeForgottenPassword;
using CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class ChangeForgottenPasswordTests
{
    private readonly UserFaker _userFaker = new();
    private readonly ICryptoService _cryptoService = Substitute.For<ICryptoService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<ChangeForgottenPasswordCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<ChangeForgottenPasswordCommandHandler>>();

    private readonly ChangeForgottenPasswordCommandHandler _sut;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ChangeForgottenPasswordTests()
    {
        _sut = new(_userRepository, _cryptoService, _logger);
    }

    [Fact]
    public async Task ShouldChangePassword_WhenInputIsValid()
    {
        // Arrange
        var user = _userFaker.Generate();
        var oldPassword = user.Password;
        var code = user.GenerateForgotPasswordCode();
        var newPassword = UserFaker.GenerateDifferentPassword(user.Email);
        var request = new ChangeForgottenPasswordCommand(user.Email, code, newPassword, newPassword);

        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();
        _userRepository.Single(spec, true, _cancellationToken).Returns(user);
        _userRepository.Save(_cancellationToken).Returns(new RepositoryResult());
        _cryptoService.Hash(newPassword).Returns(newPassword);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Email.Should().Be(user.Email);
        result.Value.UserId.Should().Be(user.Id);
        user.Password.Should().NotBe(oldPassword);
        user.Password.Should().Be(newPassword);

        _logger.Received(1).LogInformation("Changing forgotten password for {Email}", request.Email);
        _logger.Received(1).LogInformation("Successfully changed forgotten password for {Email}", request.Email);
        await _userRepository.Received(1).Save(_cancellationToken);
    }

    [Fact]
    public async Task ShouldReturnChangePasswordFailed_WhenDbFails()
    {
        // Arrange
        var user = _userFaker.Generate();
        var oldPassword = user.Password;
        var code = user.GenerateForgotPasswordCode();
        var newPassword = UserFaker.GenerateDifferentPassword(user.Email);
        const string errorMessage = "Database error";

        var request = new ChangeForgottenPasswordCommand(user.Email, code, newPassword, newPassword);

        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();
        _userRepository.Single(spec, true, _cancellationToken).Returns(user);
        _userRepository.Save(_cancellationToken).Returns(new RepositoryResult(errorMessage));
        _cryptoService.Hash(newPassword).Returns(newPassword);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ChangeForgotPassword_OperationFailed(request.Email));

        _logger.Received(1).LogInformation("Changing forgotten password for {Email}", request.Email);
        _logger.Received(1).LogError("Failed to change forgotten password for {Email}. Error: {Error}", request.Email,
            errorMessage);
        _logger.DidNotReceive().LogInformation("Successfully changed forgotten password for {Email}", request.Email);
        await _userRepository.Received(1).Save(_cancellationToken);
    }

    [Fact]
    public async Task ShouldReturnInvalidCodeError_WhenCodeIsInvalid()
    {
        // Arrange
        var user = _userFaker.Generate();
        var oldPassword = user.Password;
        var userCode = user.GenerateForgotPasswordCode();
        var inputCode = UserFaker.GenerateDifferentCode(userCode);
        var newPassword = UserFaker.GenerateDifferentPassword(user.Email);
        var request = new ChangeForgottenPasswordCommand(user.Email, inputCode, newPassword, newPassword);

        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();
        _userRepository.Single(spec, true, _cancellationToken).Returns(user);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);
        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ChangeForgotPassword_InvalidCode(request.Email));

        _logger.Received(1).LogInformation("Changing forgotten password for {Email}", request.Email);
        _logger.DidNotReceive().LogInformation("Successfully changed forgotten password for {Email}", request.Email);
        await _userRepository.DidNotReceive().Save(_cancellationToken);
    }

    [Fact]
    public async Task ShouldReturnInvalidEmailError_WhenEmailIsInvalid()
    {
        // Arrange
        var user = _userFaker.Generate();
        var oldPassword = user.Password;
        var code = user.GenerateForgotPasswordCode();
        var differentEmail = UserFaker.GenerateDifferentEmail(user.Email);
        var newPassword = UserFaker.GenerateDifferentPassword(user.Email);
        var request = new ChangeForgottenPasswordCommand(differentEmail, code, newPassword, newPassword);

        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();
        _userRepository.Single(spec, true, _cancellationToken).ReturnsNull();


        // Act
        var result = await _sut.Handle(request, _cancellationToken);
        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ChangeForgotPassword_InvalidEmail(request.Email));


        _logger.Received(1).LogInformation("Changing forgotten password for {Email}", request.Email);
        _logger.DidNotReceive().LogInformation("Successfully changed forgotten password for {Email}", request.Email);
        await _userRepository.DidNotReceive().Save(_cancellationToken);
    }
}
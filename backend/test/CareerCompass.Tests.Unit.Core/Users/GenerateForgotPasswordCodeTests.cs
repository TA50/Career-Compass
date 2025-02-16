using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Users;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class GenerateForgotPasswordCodeTests
{
    private readonly UserFaker _userFaker = new();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<GenerateForgotPasswordCodeCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<GenerateForgotPasswordCodeCommandHandler>>();

    private readonly GenerateForgotPasswordCodeCommandHandler _sut;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GenerateForgotPasswordCodeTests()
    {
        _sut = new(_userRepository, _logger);
    }

    [Fact]
    public async Task ShouldGenerateForgotPasswordCode_WhenInputIsValid()
    {
        // Arrange
        var user = _userFaker.Generate();
        var spec = new GetUserByEmailSpecification(user.Email)
            .RequireConfirmation();
        _userRepository.Single(spec, true, _cancellationToken).Returns(user);

        _userRepository.Save(_cancellationToken).Returns(new RepositoryResult());

        var request = new GenerateForgotPasswordCodeCommand(user.Email);

        // Act
        var result = await _sut.Handle(request, _cancellationToken);
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Email.Should().Be(user.Email);
        user.ForgotPasswordCode.Should().NotBeNullOrWhiteSpace();
        result.Value.Code.Should().Be(user.ForgotPasswordCode);

        await _userRepository.Received(1).Save(_cancellationToken);

        _logger.Received(1).LogInformation("Forgot password code generated for {Email}", request.Email);
        _logger.Received(1).LogInformation("Forgot password code generated successfully for {Email}", request.Email);
    }

    [Fact]
    public async Task ShouldReturnUserNotFound_WhenEmailDoesNotBelongToUser()
    {
        // Arrange
        var user = _userFaker.Generate();
        var invalidEmail = UserFaker.GenerateDifferentEmail(user.Email);
        var request = new GenerateForgotPasswordCodeCommand(invalidEmail);
        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();
        _userRepository.Single(spec, true, _cancellationToken).ReturnsNull();

        // Act
        var result = await _sut.Handle(request, _cancellationToken);
        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ForgotPassword_InvalidEmail(request.Email));

        _logger.Received(1).LogInformation("Forgot password code generated for {Email}", request.Email);
        _logger.Received(0).LogInformation("Forgot password code generated successfully for {Email}", request.Email);
    }

    [Fact]
    public async Task ShouldReturnForgotPasswordFailed_WhenDbFails()
    {
        // Arrange
        var user = _userFaker.Generate();
        const string dbError = "Db Error";
        var request = new GenerateForgotPasswordCodeCommand(user.Email);

        var spec = new GetUserByEmailSpecification(request.Email)
            .RequireConfirmation();
        _userRepository.Single(spec, true, _cancellationToken).Returns(user);
        _userRepository.Save(_cancellationToken).Returns(new RepositoryResult(dbError));

        // Act
        var result = await _sut.Handle(request, _cancellationToken);
        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.ForgotPassword_OperationFailed(request.Email));

        await _userRepository.Received(1).Save(_cancellationToken);
        _logger.Received(1).LogInformation("Forgot password code generated for {Email}", request.Email);
        _logger.Received(0).LogInformation("Forgot password code generated successfully for {Email}", request.Email);
        _logger.Received(1).LogError("Forgot password code generation failed for {Email}. Error: {Error}",
            request.Email,
            dbError);
    }
}
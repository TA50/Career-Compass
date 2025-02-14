using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.ConfirmEmail;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class ConfirmEmailCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<ConfirmEmailCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<ConfirmEmailCommandHandler>>();

    private readonly ConfirmEmailCommandHandler _sut;

    public ConfirmEmailCommandHandlerTests()
    {
        _sut = new ConfirmEmailCommandHandler(_userRepository, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserId_WhenEmailConfirmationIsValid()
    {
        // Arrange
        var user = User.Create("", "", "", "");
        var code = user.GenerateEmailConfirmationCode();
        var request = new ConfirmEmailCommand(user.Id, code);

        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.Save(Arg.Any<CancellationToken>()).Returns(new RepositoryResult());

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);


        // Assert
        _logger.Received(1).LogInformation("Confirming email for user with id {UserId}", request.UserId);
        _logger.Received(1).LogInformation("Email confirmed for user with id {UserId}", request.UserId);

        result.IsError.Should().BeFalse();
        result.Value.UserId.Should().Be(user.Id);
        user.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnUserNotFoundError_WhenUserDoesNotExist()
    {
        // Arrange
        var user = User.Create("", "", "", "");
        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).ReturnsNull();
        var request = new ConfirmEmailCommand(user.Id, "code");
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);


        // Assert
        _logger.Received(1).LogInformation("Confirming email for user with id {UserId}", request.UserId);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(
            UserErrors.UserEmailConfirmation_UserNotFound(request.UserId));
    }


    [Fact]
    public async Task Handle_ShouldReturnInvalidCodeError_WhenCodeIsNotCorrect()
    {
        // Arrange
        var user = User.Create("", "", "", "");
        user.GenerateEmailConfirmationCode();
        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>()).Returns(user);
        var request = new ConfirmEmailCommand(user.Id, "invalid_code");
        // Act
        var result = await _sut.Handle(request, CancellationToken.None);


        // Assert
        _logger.Received(1).LogInformation("Confirming email for user with id {UserId}", request.UserId);

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(
            UserErrors.UserEmailConfirmation_InvalidEmailConfirmationCode(request.UserId));
    }
}
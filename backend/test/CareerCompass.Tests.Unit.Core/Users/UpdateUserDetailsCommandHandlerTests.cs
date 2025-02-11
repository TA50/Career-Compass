using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.UpdateDetails;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class UpdateUserDetailsCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UpdateUserDetailsCommandHandler _sut;

    private readonly ILoggerAdapter<UpdateUserDetailsCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<UpdateUserDetailsCommandHandler>>();

    public UpdateUserDetailsCommandHandlerTests()
    {
        _sut = new UpdateUserDetailsCommandHandler(_userRepository, _logger);
    }

    [Fact(DisplayName = "Handle: Should return UserModification_UserNotFoundError when user does not exist")]
    public async Task Handle__ShouldReturnUserModification_UserNotFoundError__WhenUserDoesntExist()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        const string firstName = "new first name";
        const string lastName = "new last name";

        var command = new UpdateUserDetailsCommand(
            UserId: userId,
            FirstName: firstName,
            LastName: lastName
        );

        _userRepository.Get(userId, true, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating user details for user {@UserId}", userId);
        _logger.Received(1).LogError("Failed to update user details for user {@UserId}", userId);
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.UserModification_UserNotFound(userId));
        result.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnModifiedUser_WhenUserIsUpdated()
    {
        // Arrange
        const string firstName = "new first name";
        const string lastName = "new last name";

        const string oldFirstName = "new first name";
        const string oldLastName = "new last name";

        const string userName = "username";
        const string password = "password";

        var originalUser = User.Create(
            firstName: oldFirstName,
            lastName: oldLastName,
            email: userName,
            password: password
        );
        var command = new UpdateUserDetailsCommand(
            UserId: originalUser.Id,
            FirstName: firstName,
            LastName: lastName
        );


        _userRepository.Get(originalUser.Id, true, Arg.Any<CancellationToken>())
            .Returns(originalUser);

        _userRepository.Save(Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult());


        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating user details for user {@UserId}", originalUser.Id);
        _logger.Received(1).LogInformation("Updated user details for user {@UserId}", originalUser.Id);


        result.IsError.Should().BeFalse();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.Id.Should().Be(originalUser.Id);
        result.Value.Email.Should().Be(userName);
        result.Value.Password.Should().Be(password);
    }

    [Fact(DisplayName = "Handle: Should return UserModificationFailedError when DB error occured")]
    public async Task Handle_ShouldReturnUserModificationFailedError_WhenDBErrorOccured()
    {
        // Arrange
        var originalUser = User.Create(
            "username",
            "password",
            "firstname",
            "lastname"
        );

        _userRepository.Get(originalUser.Id, true, Arg.Any<CancellationToken>())
            .Returns(originalUser);
        const string dbError = "DB Error";
        _userRepository.Save(Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult(dbError));

        // Act
        var result = await _sut.Handle(new UpdateUserDetailsCommand(originalUser.Id, "new firstname", "new lastname"),
            CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Updating user details for user {@UserId}", originalUser.Id);
        _logger.Received(1).LogError("Failed to update user details for user {@UserId}", originalUser.Id);

        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.UserModification_ModificationFailed(originalUser.Id));
        result.Errors.Should().HaveCount(1);
    }
}
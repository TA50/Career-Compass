using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.UpdateDetails;
using CareerCompass.Tests.Fakers.Core;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CareerCompass.Tests.Unit.Core.Users;

public class UpdateUserDetailsTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UpdateUserDetailsCommandHandler _sut;
    private readonly UserFaker _userFaker = new();

    private readonly ILoggerAdapter<UpdateUserDetailsCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<UpdateUserDetailsCommandHandler>>();

    public UpdateUserDetailsTests()
    {
        _sut = new(_userRepository, _logger);
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
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.UserModification_UserNotFound(userId));
        result.Errors.Should().HaveCount(1);

        _logger.Received(1).LogInformation("Updating user details for user {@UserId}", userId);
    }

    [Fact]
    public async Task Handle_ShouldReturnModifiedUser_WhenUserIsUpdated()
    {
        // Arrange
        var user = _userFaker.Generate();

        var command = new UpdateUserDetailsCommand(
            UserId: user.Id,
            FirstName: "new first name",
            LastName: "new last name"
        );


        _userRepository.Get(user.Id, true, Arg.Any<CancellationToken>())
            .Returns(user);

        var repositoryResult = new RepositoryResult();
        _userRepository.Save(Arg.Any<CancellationToken>())
            .Returns(repositoryResult);


        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert


        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(user);
        user.FirstName.Should().Be(command.FirstName);
        user.LastName.Should().Be(command.LastName);
        _logger.Received(1).LogInformation("Updating user details for user {@UserId}", user.Id);
        _logger.Received(1).LogInformation("Updated user details for user {@UserId}", user.Id);
    }

    [Fact(DisplayName = "Handle: Should return UserModificationFailedError when DB error occured")]
    public async Task Handle_ShouldReturnUserModificationFailedError_WhenDBErrorOccured()
    {
        // Arrange
        var user = _userFaker.Generate();
        var cancellationToken = CancellationToken.None;

        _userRepository.Get(user.Id, true, cancellationToken)
            .Returns(user);
        const string dbError = "DB Error";
        _userRepository.Save(cancellationToken)
            .Returns(new RepositoryResult(dbError));

        // Act
        var result = await _sut.Handle(new UpdateUserDetailsCommand(user.Id, "new firstname", "new lastname"),
            CancellationToken.None);

        // Assert

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.UserModification_ModificationFailed(user.Id));
        result.Errors.Should().HaveCount(1);

        _logger.Received(1).LogInformation("Updating user details for user {@UserId}", user.Id);
        _logger.Received(1).LogError("Failed to update user details for user {@UserId}", user.Id);
    }
}
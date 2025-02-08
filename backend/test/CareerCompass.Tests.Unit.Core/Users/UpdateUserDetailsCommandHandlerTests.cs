using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Users;
using CareerCompass.Core.Users.Commands.UpdateDetails;
using CareerCompass.Tests.Unit.Core.Shared;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace CareerCompass.Tests.Unit.Core.Users;

public class UpdateUserDetailsCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UpdateUserDetailsCommandHandler _sut;

    public UpdateUserDetailsCommandHandlerTests()
    {
        _sut = new UpdateUserDetailsCommandHandler(_userRepository);
    }

    [Fact]
    public async Task Handle__ShouldReturnUserModification_UserNotFoundError__WhenUserDoesntExist()
    {
        // Arrange
        var userId = UserId.NewId();
        const string firstName = "new first name";
        const string lastName = "new last name";
        var command = new UpdateUserDetailsCommand(
            UserId: userId,
            FirstName: firstName,
            LastName: lastName
        );
        _userRepository.Get(userId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBeEquivalentToError(UserErrors.UserModification_UserNotFound(userId));
    }

    [Fact]
    public async Task Handle_ShouldReturnModifiedUser_WhenUserIsUpdated()
    {
        // Arrange
        var userId = UserId.NewId();
        const string firstName = "new first name";
        const string lastName = "new last name";
        const string email = "email";
        var command = new UpdateUserDetailsCommand(
            UserId: userId,
            FirstName: firstName,
            LastName: lastName
        );

        var originalUser = new User(
            id: userId,
            firstName: "old first name",
            lastName: "old last name",
            email: email,
            tagIds: [],
            fieldIds: [],
            scenarioIds: []
        );
        var expectedUser = new User(
            id: userId,
            firstName: firstName,
            lastName: lastName,
            email: email,
            tagIds: [],
            fieldIds: [],
            scenarioIds: []
        );
        _userRepository.Get(userId, Arg.Any<CancellationToken>())
            .Returns(originalUser);

        _userRepository.Update(originalUser, Arg.Any<CancellationToken>())
            .Returns(expectedUser);


        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEquivalentTo(expectedUser);
    }
}
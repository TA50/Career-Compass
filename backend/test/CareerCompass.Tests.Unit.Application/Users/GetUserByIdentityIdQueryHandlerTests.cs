using CareerCompass.Application.Users;
using CareerCompass.Application.Users.Queries.GetUser;
using CareerCompass.Tests.Unit.Application.Shared;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Users;

public class GetUserByIdentityIdQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetUserByIdentityIdQueryHandler _sut;

    public GetUserByIdentityIdQueryHandlerTests()
    {
        _sut = new(_userRepository);
    }

    [Fact(DisplayName = "Handle: SHOULD return user query `UserValidation_UserNotFound` error WHEN user was not found")]
    public async Task Handle__ShouldReturnUserQuery_UserNotFound__WhenUserWasNotFound()
    {
        // Arrange: 
        var identityId = UserId.NewId();

        var query = new GetUserByIdentityIdQuery(identityId.ToString());

        _userRepository.GetFromIdentity(identityId, Arg.Any<CancellationToken>()).ReturnsNull();


        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(UserErrors.UserQuery_UserNotFound(identityId));
    }

    [Fact(DisplayName = "Handle: SHOULD return user WHEN user was found")]
    public async Task Handle__ShouldReturnUser__WhenUserIsFound()
    {
        // Arrange: 
        var identityId = UserId.NewId();

        var query = new GetUserByIdentityIdQuery(identityId.ToString());

        var expectedUser = new User(
            id: identityId,
            firstName: "Test",
            email: "test@gmail.com",
            lastName: "test",
            tagIds: [],
            fieldIds: [],
            scenarioIds: []
        );
        _userRepository.GetFromIdentity(identityId, Arg.Any<CancellationToken>()).Returns(expectedUser);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEquivalentTo(expectedUser);
    }
}
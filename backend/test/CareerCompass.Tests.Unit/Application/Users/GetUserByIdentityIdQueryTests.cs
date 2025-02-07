using CareerCompass.Application.Users;
using CareerCompass.Application.Users.Queries.GetUser;
using CareerCompass.Tests.Unit.Common;
using Moq;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Users;

public class GetUserFromIdentityIdTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public GetUserFromIdentityIdTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact(DisplayName = "Given identity id that does not exist returns `UserValidation_UserNotFound` error")]
    public async Task GivenIdentityId_WhenUserNotFound_ThenReturnError()
    {
        // Arrange: 
        var identityId = UserId.NewId();

        var query = new GetUserByIdentityIdQuery(identityId.ToString())
            ;

        _userRepositoryMock.Setup(r => r.GetFromIdentity(identityId.ToString(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var useCase = new GetUserByIdentityIdQueryHandler(_userRepositoryMock.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(UserErrors.UserQuery_UserNotFound(identityId));
    }

    [Fact(DisplayName = "Given identity id that exists returns user")]
    public async Task GivenIdentityId_WhenUserFound_ThenReturnUser()
    {
        // Arrange: 
        var identityId = UserId.NewId();

        var query = new GetUserByIdentityIdQuery(identityId.ToString());

        var user = new User(id: identityId,
            firstName: "Test",
            email: "test@gmail.com",
            lastName: "test",
            tagIds: [],
            fieldIds: [],
            scenarioIds: []
        );
        _userRepositoryMock.Setup(r => r.GetFromIdentity(identityId.ToString(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var useCase = new GetUserByIdentityIdQueryHandler(_userRepositoryMock.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(user);
    }
}
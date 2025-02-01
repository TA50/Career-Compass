using CareerCompass.Application.Tags;
using CareerCompass.Application.Tags.Commands.CreateTag;
using CareerCompass.Application.Users;
using Moq;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Tags;

public class CreateTagTests
{
    private readonly Mock<ITagRepository> _tagRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();


    /**
     *  Given name and user id
     * - Should return error if user not found
     * - Should return error if tag name already exists for the (same user)
     * - Should return tag if tag created successfully
     */
    [Fact(DisplayName = "Given tag input with user id that does not exist returns `TagValidation_UserNotFound` error")]
    public async Task GivenNameAndUserId_WhenUserNotFound_ThenReturnError()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(false);

        var createdTag = new Tag(TagId.NewId(), tagName, userId);
        _tagRepository.Setup(r => r.Create(It.IsNotNull<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTag);

        var useCase = new CreateTagCommandHandler(_tagRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createTagInput, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(TagErrors.TagValidation_UserNotFound(userId));
    }

    [Fact(DisplayName =
        "Given tag input with tag name that already exists for the same user returns `TagValidation_TagNameAlreadyExists` error")]
    public async Task GivenNameAndUserId_WhenTagNameAlreadyExistsForUser_ThenReturnError()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(true);
        _tagRepository.Setup(r => r.Exists(userId, tagName, CancellationToken.None)).ReturnsAsync(true);

        var createdTag = new Tag(TagId.NewId(), tagName, userId);
        _tagRepository.Setup(r => r.Create(It.IsNotNull<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTag);

        var useCase = new CreateTagCommandHandler(_tagRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createTagInput, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(TagErrors.TagValidation_TagNameAlreadyExists(userId, tagName));
    }

    [Fact(DisplayName =
        "Given tag input with tag name that already exists but for different user it should not contain `TagValidation_TagNameAlreadyExists` error")]
    public async Task GivenNameAndUserId_WhenTagNameAlreadyExistsForDifferentUser_ThenTagIsCreated()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var anotherUserId = UserId.NewId();
        var tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(true);
        _tagRepository.Setup(r => r.Exists(anotherUserId, tagName, CancellationToken.None)).ReturnsAsync(true);
        _tagRepository.Setup(r => r.Exists(userId, tagName, CancellationToken.None)).ReturnsAsync(false);

        var createdTag = new Tag(TagId.NewId(), tagName, userId);
        _tagRepository.Setup(r => r.Create(It.IsNotNull<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTag);

        var useCase = new CreateTagCommandHandler(_tagRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createTagInput, CancellationToken.None);

        // Assert:
        result.Errors.ShouldNotContain(TagErrors.TagValidation_TagNameAlreadyExists(userId, tagName));
        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe(createTagInput.Name);
        result.Value.UserId.ShouldBe(createTagInput.UserId);
    }

    [Fact]
    public async Task GivenNameAndUserId_WhenTagInputIsValid_ThenReturnTag()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(true);
        _tagRepository.Setup(r => r.Exists(userId, tagName, CancellationToken.None)).ReturnsAsync(false);

        var createdTag = new Tag(TagId.NewId(), tagName, userId);
        _tagRepository.Setup(r => r.Create(It.IsNotNull<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTag);

        var useCase = new CreateTagCommandHandler(_tagRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createTagInput, CancellationToken.None);

        // Assert:
        result.Value.ShouldBe(createdTag);

        result.Value.Name.ShouldBe(createTagInput.Name);
        result.Value.UserId.ShouldBe(createTagInput.UserId);
    }
}
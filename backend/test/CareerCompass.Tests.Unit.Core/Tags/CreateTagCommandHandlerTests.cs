using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Tags.Commands.CreateTag;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Unit.Core.Shared;
using NSubstitute;
using Shouldly;

namespace CareerCompass.Tests.Unit.Core.Tags;

public class CreateTagCommandHandlerTests
{
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly CreateTagCommandHandler _sut;

    public CreateTagCommandHandlerTests()
    {
        _sut = new CreateTagCommandHandler(_tagRepository, _userRepository);
    }

    [Fact(DisplayName = "Handle: Should return TagCreation_UserNotFoundError if user not found")]
    public async Task Handle__ShouldReturnTagCreation_UserNotFoundError__WhenUserWasNotFound()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act: 
        var result = await _sut.Handle(createTagInput, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(TagErrors.TagCreation_UserNotFound(userId));
    }

    [Fact(DisplayName =
        "Handle: Should return TagCreation_TagNameAlreadyExists if tag name already exists for the same user")]
    public async Task Handle__ShouldReturnTagCreation_TagNameAlreadyExists__WhenTagAlreadyExistsForTheGivenUser()
    {
        // Arrange: 
        var userId = UserId.NewId();
        const string tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _tagRepository.Exists(userId, tagName, Arg.Any<CancellationToken>()).Returns(true);

        // Act: 
        var result = await _sut.Handle(createTagInput, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(TagErrors.TagCreation_TagNameAlreadyExists(userId, tagName));
    }


    [Fact(DisplayName = "Handle: Should return created tag if tag created successfully")]
    public async Task Handle_ShouldReturnCreatedTag_WhenInputIsValid()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        _tagRepository.Exists(userId, tagName, Arg.Any<CancellationToken>()).Returns(false);

        var createdTag = new Tag(TagId.NewId(), tagName, userId);
        _tagRepository.Create(Arg.Do<Tag>(x => createdTag = x), Arg.Any<CancellationToken>())
            .Returns(info => info.Arg<Tag>());
        
        // Act: 
        var result = await _sut.Handle(createTagInput, CancellationToken.None);

        
        // Assert:
        result.Value.ShouldBe(createdTag);
        result.Value.Name.ShouldBe(createTagInput.Name);
        result.Value.UserId.ShouldBe(createTagInput.UserId);
    }
}
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Tags.Commands.Create;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Tags;

public class CreateTagCommandHandlerTests
{
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<CreateTagCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<CreateTagCommandHandler>>();

    private readonly CreateTagCommandHandler _sut;

    public CreateTagCommandHandlerTests()
    {
        _sut = new CreateTagCommandHandler(_tagRepository, _userRepository, _logger);
    }

    [Fact(DisplayName = "Handle: Should return TagCreation_UserNotFoundError if user not found")]
    public async Task Handle__ShouldReturnTagCreation_UserNotFoundError__WhenUserWasNotFound()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act: 
        var result = await _sut.Handle(createTagInput, CancellationToken.None);

        // Assert:

        _logger.Received(1).LogInformation("Creating tag for user {@UserId} {@TagName}", userId, tagName);

        var expectedError = TagErrors.TagCreation_UserNotFound(userId);
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.ShouldBeEquivalentTo(expectedError);
    }

    [Fact(DisplayName =
        "Handle: Should return TagCreation_TagNameAlreadyExists if tag name already exists for the same user")]
    public async Task Handle__ShouldReturnTagCreation_TagNameAlreadyExists__WhenTagAlreadyExistsForTheGivenUser()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        var spec = new GetTagByNameSpecification(userId, tagName);
        _tagRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(true);


        // Act: 
        var result = await _sut.Handle(createTagInput, CancellationToken.None);

        // Assert:

        _logger.Received(1).LogInformation("Creating tag for user {@UserId} {@TagName}", userId, tagName);


        var expectedError = TagErrors.TagCreation_TagNameAlreadyExists(userId, tagName);
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.ShouldBeEquivalentTo(expectedError);
    }


    [Fact(DisplayName = "Handle: Should return created tag if tag created successfully")]
    public async Task Handle_ShouldReturnCreatedTag_WhenInputIsValid()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string tagName = "test tag name";
        var createdTag = Tag.Create(userId, tagName);
        var request = new CreateTagCommand(userId, tagName);

        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        var spec = new GetTagByNameSpecification(userId, tagName);
        _tagRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(false);


        _tagRepository.Create(Arg.Do<Tag>(x => createdTag = x), Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult());

        // Act: 
        var result = await _sut.Handle(request, CancellationToken.None);


        // Assert:
        _logger.Received(1).LogInformation("Creating tag for user {@UserId} {@TagName}", request.UserId, request.Name);
        _logger.Received(1).LogInformation("Tag created for user {@UserId} {@TagName}", request.UserId, request.Name);
        result.Value.Should().BeEquivalentTo(createdTag);
        result.Value.Name.Should().Be(request.Name);
        result.Value.UserId.Should().Be(request.UserId);
    }


    [Fact(DisplayName = "Handle: Should return TagCreation_FailedToCreateTag if tag creation failed by the database")]
    public async Task Handle_ShouldReturnTagCreation_FailedToCreateTag__WhenTagCreationFailedFromDB()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string tagName = "test tag name";
        var createTagInput = new CreateTagCommand(userId, tagName);
        const string dbErrorMessage = "DB Error Message";
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);
        var spec = new GetTagByNameSpecification(userId, tagName);
        _tagRepository.Exists(spec, Arg.Any<CancellationToken>()).Returns(false);


        _tagRepository.Create(Arg.Is<Tag>(x => x.UserId == userId && x.Name == tagName),
                Arg.Any<CancellationToken>())
            .Returns(new RepositoryResult(dbErrorMessage));

        // Act: 
        var result = await _sut.Handle(createTagInput, CancellationToken.None);


        // Assert:
        _logger.Received(1).LogInformation("Creating tag for user {@UserId} {@TagName}", userId, tagName);
        _logger.Received(1).LogError(
            "Failed to create tag for user {@UserId} {@TagName}. Failed with message: {@Message}",
            userId, tagName, dbErrorMessage);

        var expectedError = TagErrors.TagCreation_FailedToCreateTag(userId, tagName);
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.FirstError.ShouldBeEquivalentTo(expectedError);
    }
}
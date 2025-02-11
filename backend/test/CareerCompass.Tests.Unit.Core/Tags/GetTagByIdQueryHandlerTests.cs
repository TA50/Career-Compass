using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Tags.Queries.GetTagByIdQuery;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;


namespace CareerCompass.Tests.Unit.Core.Tags;

public class GetTagByIdQueryHandlerTests
{
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly GetTagByIdQueryHandler _sut;

    private readonly ILoggerAdapter<GetTagByIdQueryHandler> _logger =
        Substitute.For<ILoggerAdapter<GetTagByIdQueryHandler>>();

    public GetTagByIdQueryHandlerTests()
    {
        _sut = new GetTagByIdQueryHandler(_tagRepository, _logger);
    }


    [Fact(DisplayName = "Handle: Should return tag")]
    public async Task Handle_ShouldReturnTags_WhenTagsExist()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string tagName = "test tag name";
        var expectedTag = Tag.Create(userId, tagName);

        var query = new GetTagByIdQuery(userId, expectedTag.Id);
        var spec = new GetTagByIdSpecification(expectedTag.Id, userId);

        _tagRepository.Single(spec, Arg.Any<CancellationToken>()).Returns(expectedTag);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        _logger.Received(1).LogInformation("Getting tag for user {@UserId} {@TagId}", userId, expectedTag.Id);
        _logger.Received(1).LogInformation("Found tag for user {@UserId} {@TagId}", userId, expectedTag.Id);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(expectedTag);
    }

    [Fact(DisplayName = "Handle: Should return TagRead_TagNotFound when tag was not found")]
    public async Task Handle_ShouldReturnEmptyList_WhenNotTagsExist()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var tagId = TagId.CreateUnique();
        var query = new GetTagByIdQuery(userId, tagId);
        var spec = new GetTagByIdSpecification(tagId, userId);
        _tagRepository.Get(spec, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        _logger.Received(1).LogInformation("Getting tag for user {@UserId} {@TagId}", userId, tagId);

        var expectedError = TagErrors.TagRead_TagNotFound(userId, tagId);
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(expectedError);
        result.Errors.Should().HaveCount(1);
    }
}
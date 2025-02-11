using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Tags;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Tags.Queries.GetTagsQuery;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Tags;

public class GetTagsQueryHandlerTests
{
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly GetTagsQueryHandler _sut;

    private readonly ILoggerAdapter<GetTagsQueryHandler> _logger =
        Substitute.For<ILoggerAdapter<GetTagsQueryHandler>>();

    public GetTagsQueryHandlerTests()
    {
        _sut = new(_tagRepository, _logger);
    }


    [Fact(DisplayName = "Handle: Should return list of Tags")]
    public async Task Handle_ShouldReturnTags_WhenTagsExist()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string tagName = "test tag name";
        var query = new GetTagsQuery(userId);
        var expectedTag = Tag.Create(userId, tagName);
        List<Tag> tags = [expectedTag];

        var spec = new GetUserTagsSpecification(userId);
        _tagRepository.Get(spec, Arg.Any<CancellationToken>()).Returns(tags);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        const int expectedCount = 0;
        _logger.Received(1).LogInformation("Getting tags for user {UserId}", userId);
        _logger.Received(1).LogInformation("Found {TagsCount} tags for user {@UserId}", expectedCount, userId);

        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(expectedCount);
        result.Value.Single().Should().BeEquivalentTo(expectedTag);
    }

    [Fact(DisplayName = "Handle: Should return empty list when no tags exist")]
    public async Task Handle_ShouldReturnEmptyList_WhenNotTagsExist()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var query = new GetTagsQuery(userId);
        var spec = new GetUserTagsSpecification(userId);
        _tagRepository.Get(spec, Arg.Any<CancellationToken>()).Returns([]);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        var expectedCount = 0;
        _logger.Received(1).LogInformation("Getting tags for user {UserId}", userId);
        _logger.Received(1).LogInformation("Found {TagsCount} tags for user {@UserId}", 1, userId);


        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(expectedCount);
        result.Value.Should().BeEmpty();
    }
}
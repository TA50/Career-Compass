using CareerCompass.Application.Tags;
using CareerCompass.Application.Tags.Queries.GetTagByIdQuery;
using CareerCompass.Application.Tags.Queries.GetTagsQuery;
using CareerCompass.Application.Users;
using CareerCompass.Tests.Unit.Application.Shared;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Tags;

public class GetTagByIdQueryHandlerTests
{
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly GetTagByIdQueryHandler _sut;

    public GetTagByIdQueryHandlerTests()
    {
        _sut = new(_tagRepository);
    }


    [Fact(DisplayName = "Handle: Should return tag")]
    public async Task Handle_ShouldReturnTags_WhenTagsExist()
    {
        // Arrange: 
        var userId = UserId.NewId();
        const string tagName = "test tag name";
        var tagId = TagId.NewId();
        var query = new GetTagByIdQuery(userId, tagId);
        var expectedTag = new Tag(TagId.NewId(), tagName + "1", userId);


        _tagRepository.Get(userId, tagId, Arg.Any<CancellationToken>()).Returns(expectedTag);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEquivalentTo(expectedTag);
    }

    [Fact(DisplayName = "Handle: Should return TagRead_TagNotFound when tag was not found")]
    public async Task Handle_ShouldReturnEmptyList_WhenNotTagsExist()
    {
        // Arrange: 
        var userId = UserId.NewId();
        const string name = "test name";
        var tagId = TagId.NewId();
        var query = new GetTagByIdQuery(userId, tagId);

        _tagRepository.Get(userId, tagId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        var expectedError = TagErrors.TagRead_TagNotFound(userId, tagId);
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBeEquivalentToError(expectedError);
    }
}
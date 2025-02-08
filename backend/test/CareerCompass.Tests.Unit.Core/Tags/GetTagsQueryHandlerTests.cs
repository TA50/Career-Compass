using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Tags;
using CareerCompass.Core.Tags.Queries.GetTagByIdQuery;
using CareerCompass.Core.Tags.Queries.GetTagsQuery;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Unit.Core.Shared;
using NSubstitute;
using Shouldly;

namespace CareerCompass.Tests.Unit.Core.Tags;

public class GetTagsQueryHandlerTests
{
    private readonly ITagRepository _tagRepository = Substitute.For<ITagRepository>();
    private readonly GetTagsQueryHandler _sut;

    public GetTagsQueryHandlerTests()
    {
        _sut = new(_tagRepository);
    }


    [Fact(DisplayName = "Handle: Should return list of Tags")]
    public async Task Handle_ShouldReturnTags_WhenTagsExist()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagName = "test tag name";
        var query = new GetTagsQuery(userId);
        var expectedTag = new Tag(TagId.NewId(), tagName + "1", userId);
        List<Tag> tags = [expectedTag];


        _tagRepository.Get(userId, Arg.Any<CancellationToken>()).Returns(tags);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldHaveSingleItem();
        result.Value.Single().ShouldBeEquivalentTo(expectedTag);
    }

    [Fact(DisplayName = "Handle: Should return empty list when no tags exist")]
    public async Task Handle_ShouldReturnEmptyList_WhenNotTagsExist()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var name = "test name";
        var fieldId = TagId.NewId();
        var query = new GetTagsQuery(userId);


        _tagRepository.Get(userId, Arg.Any<CancellationToken>()).Returns(Enumerable.Empty<Tag>().ToList());

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEmpty();
    }
}
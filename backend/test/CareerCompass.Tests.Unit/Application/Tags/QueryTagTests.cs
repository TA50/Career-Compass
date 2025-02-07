using CareerCompass.Application.Tags;
using CareerCompass.Application.Tags.Queries.GetTagByIdQuery;
using CareerCompass.Application.Tags.Queries.GetTagsQuery;
using CareerCompass.Application.Users;
using CareerCompass.Tests.Unit.Common;
using Moq;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Tags;

public class QueryTagTests
{
    private readonly Mock<ITagRepository> _tagRepository = new();


    /**
     *  Given user id
     * - Should return list of Tags
     */
    [Fact(DisplayName = "Given the user id it should return the list of tags")]
    public async Task GivenUserId_ThenReturnTags()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagName = "test tag name";
        var query = new GetTagsQuery(userId);

        List<Tag> tags =
        [
            new(TagId.NewId(), tagName + "1", userId),
            new(TagId.NewId(), tagName + "2", userId),
            new(TagId.NewId(), tagName + "3", userId)
        ];


        _tagRepository.Setup(r => r.Get(userId, CancellationToken.None))
            .ReturnsAsync(tags);

        var useCase = new GetTagsQueryHandler(_tagRepository.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(tags.Count);
        foreach (var tag in tags)
        {
            result.Value.ShouldContain(tag);
        }
    }

    /**
      *  Given user id and Tag Id
      * - Should return  Tag
      */
    [Fact(DisplayName = "Given the user id and field id it should return the field")]
    public async Task GivenUserIdAndTagId_ThenReturnTag()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var name = "test name";
        var fieldId = TagId.NewId();
        var query = new GetTagByIdQuery(userId, fieldId);


        _tagRepository.Setup(r => r.Get(userId, fieldId, CancellationToken.None))
            .ReturnsAsync(new Tag(fieldId, name, userId));

        var useCase = new GetTagByIdQueryHandler(_tagRepository.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.Id.ShouldBe(fieldId);
        result.Value.UserId.ShouldBe(userId);
        result.Value.Name.ShouldBe(name);
    }

    /**
       *  Given user id and Tag Id that does not exist
       * - Should return  TagRead Tag not found Error
       */
    [Fact(DisplayName = "Given the user id and tag id which do not exist it should return not found error")]
    public async Task GivenUserIdAndTagId_WhenTagDontExists_ThenReturnError()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var tagId = TagId.NewId();
        var query = new GetTagByIdQuery(userId, tagId);

        _tagRepository.Setup(r => r.Get(userId, tagId, CancellationToken.None))
            .ReturnsAsync((Tag?)null);

        var useCase = new GetTagByIdQueryHandler(_tagRepository.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(TagErrors.TagRead_TagNotFound(userId, tagId));
    }
}
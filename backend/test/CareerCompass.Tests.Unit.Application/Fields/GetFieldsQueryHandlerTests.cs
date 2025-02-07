using CareerCompass.Application.Fields;
using CareerCompass.Application.Fields.Queries.GetFieldsQuery;
using CareerCompass.Application.Users;
using NSubstitute;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Fields;

public class GetFieldsQueryHandlerTests
{
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly GetFieldsQueryHandler _sut;

    public GetFieldsQueryHandlerTests()
    {
        _sut = new GetFieldsQueryHandler(_fieldRepository);
    }
    
    [Fact(DisplayName = "Handle: Should return list of Fields")]
    public async Task Handle_ShouldReturnFields_WhenFieldsExist()
    {
        // Arrange: 
        var userId = UserId.NewId();
        const string name = "test name";
        var query = new GetFieldsQuery(userId);
        var expectedField = new Field(FieldId.NewId(), name + "1", userId);
        List<Field> fields =
        [
            expectedField
        ];

        _fieldRepository.Get(userId, Arg.Any<CancellationToken>()).Returns(fields);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        result.IsError.ShouldBeFalse();
        result.Value.ShouldHaveSingleItem();
        result.Value.Single().ShouldBeEquivalentTo(expectedField);
    }

    [Fact(DisplayName = "Handle: Should return empty list when no fields exist")]
    public async Task Handle_ShouldReturnEmptyList_WhenNoFieldsExist()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var query = new GetFieldsQuery(userId);

        _fieldRepository.Get(userId).Returns(Enumerable.Empty<Field>().ToList());

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEmpty();
    }
}
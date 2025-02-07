using CareerCompass.Application.Fields;
using CareerCompass.Application.Fields.Queries.GetFieldByIdQuery;
using CareerCompass.Application.Users;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;
using CareerCompass.Tests.Unit.Application.Shared;

namespace CareerCompass.Tests.Unit.Application.Fields;

public class GetFieldByIdQueryHandlerTests
{
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly GetFieldByIdQueryHandler _sut;

    public GetFieldByIdQueryHandlerTests()
    {
        _sut = new GetFieldByIdQueryHandler(_fieldRepository);
    }

    [Fact(DisplayName = "Handle: Should return Field when field with the given id exists")]
    public async Task Handle_ShouldReturnField_WhenFieldExist()
    {
        // Arrange: 
        var userId = UserId.NewId();
        const string name = "test name";
        var fieldId = FieldId.NewId();
        var query = new GetFieldByIdQuery(userId, fieldId);
        var expectedField = new Field(fieldId, name + "1", userId);

        _fieldRepository.Get(userId, fieldId, Arg.Any<CancellationToken>()).Returns(expectedField);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBeEquivalentTo(expectedField);
    }

    [Fact(DisplayName = "Handle: Should return FieldRead_FieldNotFound when field doesn't exist")]
    public async Task Handle_ShouldReturnFieldRead_FieldNotFound_WhenFieldDoesntExists()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldId = FieldId.NewId();
        var query = new GetFieldByIdQuery(userId, fieldId);

        _fieldRepository.Get(userId, fieldId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBeEquivalentToError(FieldErrors.FieldRead_FieldNotFound(userId, fieldId));
        result.Errors.ShouldHaveSingleItem();
    }
}
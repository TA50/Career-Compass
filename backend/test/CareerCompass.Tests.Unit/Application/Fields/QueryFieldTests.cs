using CareerCompass.Application.Fields;
using CareerCompass.Application.Fields.Queries.GetFieldByIdQuery;
using CareerCompass.Application.Fields.Queries.GetFieldsQuery;
using CareerCompass.Application.Users;
using CareerCompass.Tests.Unit.Common;
using Moq;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Fields;

public class QueryFieldTests
{
    private readonly Mock<IFieldRepository> _fieldRepository = new();


    /**
     *  Given user id
     * - Should return list of Fields
     */
    [Fact(DisplayName = "Given the user id it should return the list of fields")]
    public async Task GivenUserId_ThenReturnFields()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var name = "test name";
        var query = new GetFieldsQuery(userId);

        List<Field> fields =
        [
            new(FieldId.NewId(), name + "1", userId),
            new(FieldId.NewId(), name + "2", userId),
            new(FieldId.NewId(), name + "3", userId)
        ];


        _fieldRepository.Setup(r => r.Get(userId, CancellationToken.None))
            .ReturnsAsync(fields);

        var useCase = new GetFieldsQueryHandler(_fieldRepository.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(fields.Count);
        foreach (var tag in fields)
        {
            result.Value.ShouldContain(tag);
        }
    }

    /**
       *  Given user id and Field Id
       * - Should return  Field
       */
    [Fact(DisplayName = "Given the user id and field id it should return the field")]
    public async Task GivenUserIdAndFieldId_ThenReturnField()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var name = "test name";
        var fieldId = FieldId.NewId();
        var query = new GetFieldByIdQuery(userId, fieldId);


        _fieldRepository.Setup(r => r.Get(userId, fieldId, CancellationToken.None))
            .ReturnsAsync(new Field(fieldId, name, userId));

        var useCase = new GetFieldByIdQueryHandler(_fieldRepository.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeFalse();
        result.Value.Id.ShouldBe(fieldId);
        result.Value.UserId.ShouldBe(userId);
        result.Value.Name.ShouldBe(name);
    }

    /**
       *  Given user id and Field Id that does not exists
       * - Should return  FieldRead Field not found Error
       */
    [Fact(DisplayName = "Given the user id and field id which do not exist it should return not found error")]
    public async Task GivenUserIdAndFieldId_WhenFieldDontExists_ThenReturnError()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldId = FieldId.NewId();
        var query = new GetFieldByIdQuery(userId, fieldId);
        
        _fieldRepository.Setup(r => r.Get(userId, fieldId, CancellationToken.None))
            .ReturnsAsync((Field?)null);

        var useCase = new GetFieldByIdQueryHandler(_fieldRepository.Object);
        // Act: 
        var result = await useCase.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContainError(FieldErrors.FieldRead_FieldNotFound(userId, fieldId));
    }
}
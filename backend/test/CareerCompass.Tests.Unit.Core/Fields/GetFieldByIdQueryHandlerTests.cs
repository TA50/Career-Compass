using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Fields.Queries.GetFieldByIdQuery;
using CareerCompass.Core.Users;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using FluentAssertions;

namespace CareerCompass.Tests.Unit.Core.Fields;

public class GetFieldByIdQueryHandlerTests
{
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();

    private readonly ILoggerAdapter<GetFieldByIdQueryHandler> _logger =
        Substitute.For<ILoggerAdapter<GetFieldByIdQueryHandler>>();

    private readonly GetFieldByIdQueryHandler _sut;

    public GetFieldByIdQueryHandlerTests()
    {
        _sut = new GetFieldByIdQueryHandler(_fieldRepository, _logger);
    }

    [Fact(DisplayName = "Handle: Should return Field when field with the given id exists")]
    public async Task Handle_ShouldReturnField_WhenFieldExist()
    {
        // Arrange: 

        var userId = UserId.CreateUnique();
        const string name = "Field Name";
        const string group = "Field group";
        var expectedField = Field.Create(userId, name, group);
        var spec = new GetFieldByIdSpecification(expectedField.Id, userId);
        var query = new GetFieldByIdQuery(userId, expectedField.Id);

        _fieldRepository.Single(spec, Arg.Any<CancellationToken>())
            .Returns(expectedField);
        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedField);

        _logger.Received(1).LogInformation("Getting field for user {@UserId} {@FieldId}", query.UserId, query.FieldId);
    }

    [Fact(DisplayName = "Handle: Should return FieldRead_FieldNotFound when field doesn't exist")]
    public async Task Handle_ShouldReturnFieldRead_FieldNotFound_WhenFieldDoesntExists()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var fieldId = FieldId.CreateUnique();
        var query = new GetFieldByIdQuery(userId, fieldId);

        _fieldRepository.Get(new GetFieldByIdSpecification(fieldId, userId), Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:

        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(FieldErrors.FieldRead_FieldNotFound(userId, fieldId));
        result.Errors.Should().HaveCount(1);
        _logger.Received(1).LogInformation("Getting field for user {@UserId} {@FieldId}", query.UserId, query.FieldId);
    }
}
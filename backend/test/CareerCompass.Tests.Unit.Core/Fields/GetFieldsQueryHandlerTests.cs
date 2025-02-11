using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Specifications.Fields;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Fields.Queries.GetFieldsQuery;
using CareerCompass.Core.Users;
using FluentAssertions;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Fields;

public class GetFieldsQueryHandlerTests
{
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly GetFieldsQueryHandler _sut;

    private readonly ILoggerAdapter<GetFieldsQueryHandler> _logger =
        Substitute.For<ILoggerAdapter<GetFieldsQueryHandler>>();

    public GetFieldsQueryHandlerTests()
    {
        _sut = new GetFieldsQueryHandler(_fieldRepository, _logger);
    }

    [Fact(DisplayName = "Handle: Should return list of Fields")]
    public async Task Handle_ShouldReturnFields_WhenFieldsExist()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        const string name = "test name";
        const string group = "test group";
        var query = new GetFieldsQuery(userId);
        var expectedField = Field.Create(userId, name, group);
        List<Field> fields =
        [
            expectedField
        ];
        var spec = new GetUserFieldsSpecification(userId);
        _fieldRepository.Get(spec, Arg.Any<CancellationToken>()).Returns(fields);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        _logger.Received(1).LogInformation("Getting fields for user {@UserId}", userId);

        result.IsError.Should().BeFalse();
        result.Value.Should().HaveCount(1);
        result.Value.Single().Should().BeEquivalentTo(expectedField);
    }

    [Fact(DisplayName = "Handle: Should return empty list when no fields exist")]
    public async Task Handle_ShouldReturnEmptyList_WhenNoFieldsExist()
    {
        // Arrange: 
        var userId = UserId.CreateUnique();
        var query = new GetFieldsQuery(userId);

        var spec = new GetUserFieldsSpecification(userId);
        _fieldRepository.Get(spec, Arg.Any<CancellationToken>()).Returns([]);

        // Act: 
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert:
        _logger.Received(1).LogInformation("Getting fields for user {@UserId}", userId);

        result.IsError.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}
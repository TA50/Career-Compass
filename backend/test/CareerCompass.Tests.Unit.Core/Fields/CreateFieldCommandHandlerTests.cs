using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Fields;
using CareerCompass.Core.Fields.Commands.CreateField;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Unit.Core.Shared;
using NSubstitute;
using Shouldly;

namespace CareerCompass.Tests.Unit.Core.Fields;

public class CreateFieldCommandHandlerTests
{
    private readonly IFieldRepository _fieldRepository = Substitute.For<IFieldRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly CreateFieldCommandHandler _sut;

    public CreateFieldCommandHandlerTests()
    {
        _sut = new CreateFieldCommandHandler(_fieldRepository, _userRepository);
    }

    [Fact(DisplayName = "Handle: SHOULD return FieldValidation_UserNotFound Error WHEN user was not found")]
    public async Task Handle__ShouldReturnFieldValidation_UserNotFound__WhenUserWasNotFound()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldName = "test field name";
        var createFieldInput = new CreateFieldCommand(userId, fieldName);

        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(false);
        // Act: 
        var result = await _sut.Handle(createFieldInput, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.FirstError
            .ShouldBeEquivalentToError(FieldErrors.FieldValidation_UserNotFound(userId));
    }

    [Fact(DisplayName = "Handle: SHOULD return FieldValidation_NameAlreadyExists Error WHEN field name already exists")]
    public async Task Handle__ShouldReturnFieldValidation_NameAlreadyExistsError__WhenFieldNameAlreadyExists()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldName = "test field name";
        var createFieldInput = new CreateFieldCommand(userId, fieldName);
        _fieldRepository.Exists(Arg.Any<UserId>(), fieldName, Arg.Any<CancellationToken>()).Returns(true);
        _userRepository.Exists(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act: 
        var result = await _sut.Handle(createFieldInput, CancellationToken.None);

        // Assert:
        result.IsError.ShouldBeTrue();
        result.FirstError
            .ShouldBeEquivalentToError(FieldErrors.FieldValidation_NameAlreadyExists(userId, fieldName));
    }

    [Fact(DisplayName = "Handle: SHOULD return Field WHEN CreateFieldInput is valid")]
    public async Task Handle__ShouldReturnField_WhenCreateFieldInputIsValid()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldName = "test field name";
        var createFieldInput = new CreateFieldCommand(userId, fieldName);

        _fieldRepository.Exists(userId, fieldName, Arg.Any<CancellationToken>()).Returns(false);
        _userRepository.Exists(userId, Arg.Any<CancellationToken>()).Returns(true);

        var createdField = new Field(FieldId.NewId(), fieldName, userId);

        _fieldRepository.Create(
                Arg.Do<Field>(x => createdField = x),
                Arg.Any<CancellationToken>())
            .Returns(info => info.Arg<Field>());

        // Act: 
        var result = await _sut.Handle(createFieldInput, CancellationToken.None);

        // Assert:
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(createdField);
        result.Value.Name.ShouldBe(createFieldInput.Name);
        result.Value.UserId.ShouldBe(createFieldInput.UserId);
    }
}
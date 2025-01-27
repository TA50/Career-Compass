using CareerCompass.Application.Fields;
using CareerCompass.Application.Fields.UseCases;
using CareerCompass.Application.Fields.UseCases.Contracts;
using CareerCompass.Application.Users;
using Moq;
using Shouldly;

namespace CareerCompass.Tests.Unit.Application.Fields;

public class CreateFieldTests
{
    private readonly Mock<IFieldRepository> _fieldRepository = new();
    private readonly Mock<IUserRepository> _userRepository = new();


    /**
     *  Given name and user id
     * - Should return error if user not found
     * - Should return error if field name already exists for the (same user)
     * - Should return field if field created successfully
     */
    [Fact(DisplayName =
        "Given field input with user id that does not exist returns `FieldValidation_UserNotFound` error")]
    public async Task GivenNameAndUserId_WhenUserNotFound_ThenReturnUserNotFoundError()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldName = "test field name";
        var createFieldInput = new CreateFieldInput(userId, fieldName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(false);

        var createdField = new Field(FieldId.NewId(), fieldName, userId);

        _fieldRepository.Setup(r => r.Create(It.IsNotNull<Field>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdField);

        var useCase = new CreateFieldUseCase(_fieldRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createFieldInput, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(FieldErrors.FieldValidation_UserNotFound(userId));
    }

    [Fact(DisplayName =
        "Given field input with name that already exists for the same user returns `FieldValidation_NameAlreadyExists` error")]
    public async Task GivenNameAndUserId_WhenFieldNameAlreadyExistsForUser_ThenReturnNameAlreadyExistsError()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldName = "test field name";
        var createFieldInput = new CreateFieldInput(userId, fieldName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(true);
        _fieldRepository.Setup(r => r.Exists(userId, fieldName, CancellationToken.None)).ReturnsAsync(true);

        var createdField = new Field(FieldId.NewId(), fieldName, userId);
        _fieldRepository.Setup(r => r.Create(It.IsNotNull<Field>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdField);

        var useCase = new CreateFieldUseCase(_fieldRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createFieldInput, CancellationToken.None);

        // Assert:

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(FieldErrors.FieldValidation_NameAlreadyExists(userId, fieldName));
    }

    [Fact(DisplayName =
        "Given field input with field name that already exists but for different user it should not contain `FieldValidation_NameAlreadyExists` error")]
    public async Task GivenNameAndUserId_WhenFieldNameAlreadyExistsForDifferentUser_ThenFieldIsCreated()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var anotherUserId = UserId.NewId();
        var fieldName = "test field name";
        var createFieldInput = new CreateFieldInput(userId, fieldName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(true);
        _fieldRepository.Setup(r => r.Exists(anotherUserId, fieldName, CancellationToken.None)).ReturnsAsync(true);
        _fieldRepository.Setup(r => r.Exists(userId, fieldName, CancellationToken.None)).ReturnsAsync(false);

        var createdField = new Field(FieldId.NewId(), fieldName, userId);
        _fieldRepository.Setup(r => r.Create(It.IsNotNull<Field>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdField);

        var useCase = new CreateFieldUseCase(_fieldRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createFieldInput, CancellationToken.None);

        // Assert:
        result.Errors.ShouldNotContain(FieldErrors.FieldValidation_NameAlreadyExists(userId, fieldName));
        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe(createFieldInput.Name);
        result.Value.UserId.ShouldBe(createFieldInput.UserId);
    }

    [Fact]
    public async Task GivenNameAndUserId_WhenFieldInputIsValid_ThenReturnField()
    {
        // Arrange: 
        var userId = UserId.NewId();
        var fieldName = "test field name";
        var createFieldInput = new CreateFieldInput(userId, fieldName);

        _userRepository.Setup(r => r.Exists(userId, CancellationToken.None)).ReturnsAsync(true);
        _fieldRepository.Setup(r => r.Exists(userId, fieldName, CancellationToken.None)).ReturnsAsync(false);

        var createdField = new Field(FieldId.NewId(), fieldName, userId);
        _fieldRepository.Setup(r => r.Create(It.IsNotNull<Field>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdField);

        var useCase = new CreateFieldUseCase(_fieldRepository.Object, _userRepository.Object);
        // Act: 
        var result = await useCase.Handle(createFieldInput, CancellationToken.None);

        // Assert:
        result.Value.ShouldBe(createdField);

        result.Value.Name.ShouldBe(createFieldInput.Name);
        result.Value.UserId.ShouldBe(createFieldInput.UserId);
    }
}
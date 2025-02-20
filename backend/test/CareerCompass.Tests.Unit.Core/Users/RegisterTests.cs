using CareerCompass.Core.Common;
using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Users.Commands.Register;
using NSubstitute;
using CareerCompass.Core.Common.Specifications.Users;
using CareerCompass.Core.Users;
using CareerCompass.Tests.Fakers.Core;
using ErrorOr;
using FluentAssertions;
using FluentAssertions.Primitives;


namespace CareerCompass.Tests.Unit.Core.Users;

public class RegisterTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ICryptoService _cryptoService = Substitute.For<ICryptoService>();
    private readonly RegisterCommandHandler _sut;

    private readonly RegisterCommandFaker _faker = new();


    private readonly ILoggerAdapter<RegisterCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<RegisterCommandHandler>>();


    public RegisterTests()
    {
        var settings = new CoreSettings
        {
            EmailConfirmationCodeLifetimeInHours = 6,
            ForgotPasswordCodeLifetimeInHours = 6
        };
        _sut = new RegisterCommandHandler(_userRepository, _cryptoService, settings, _logger);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserIdEmailAndConfirmationCode_WhenInputIsValid()
    {
        // Arrange
        var request = _faker.Generate();
        _cryptoService.Hash(request.Password).Returns(request.Password);
        _userRepository.Exists(new GetUserByEmailSpecification(request.Email), Arg.Any<CancellationToken>())
            .Returns(false);


        var createdUser = User.Create("", "", "", "");

        _userRepository.Create(
            Arg.Is<User>(x =>
                x.Email == request.Email
                && x.FirstName == request.FirstName
                && x.LastName ==
                request.LastName
                && x.Password == request.Password
            ), Arg.Any<CancellationToken>()).Returns(info =>
        {
            createdUser = info.Arg<User>();
            return new RepositoryResult();
        });

        // Act


        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        _logger.Received(1).LogInformation("Creating user with email {Email}", request.Email);
        _logger.Received(1).LogInformation("User with email {Email} was successfully created", request.Email);


        result.IsError.Should().BeFalse();
        result.Value.Email.Should().Be(request.Email);
        result.Value.UserId.Should().Be(createdUser.Id);
        result.Value.ConfirmationCode.Should().Be(createdUser.EmailConfirmationCode);
        createdUser.EmailConfirmed.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnUserAlreadyExistsError_WhenUserWithGivenEmailExists()
    {
        // Arrange
        var request = _faker.Generate();
        var spec = new GetUserByEmailSpecification(request.Email);
        _userRepository.Exists(Arg.Is(spec),
                Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);


        // Assert
        _logger.Received(1).LogInformation("Creating user with email {Email}", request.Email);
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.UserCreation_UserWithEmailExists(request.Email));
    }

    [Fact]
    public async Task Handle_ShouldReturnUserCreationFailedError_WhenDbFail()
    {
        // Arrange
        var request = _faker.Generate();
        _cryptoService.Hash(request.Password).Returns(request.Password);
        _userRepository.Exists(new GetUserByEmailSpecification(request.Email), Arg.Any<CancellationToken>())
            .Returns(false);

        const string dbError = "db error";
        _userRepository.Create(
            Arg.Is<User>(x =>
                x.Email == request.Email
                && x.FirstName == request.FirstName
                && x.LastName ==
                request.LastName
                && x.Password == request.Password
            ), Arg.Any<CancellationToken>()).Returns(new RepositoryResult(dbError));

        // Act

        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert

        _logger.Received(1).LogInformation("Creating user with email {Email}", request.Email);
        _logger.Received(1).LogError("Failed to create user with email {Email}: {ErrorMessage}", request.Email,
            dbError);
        result.IsError.Should().BeTrue();
        result.FirstError.ShouldBeEquivalentTo(UserErrors.UserCreation_CreationFailed(request.Email));
    }
}
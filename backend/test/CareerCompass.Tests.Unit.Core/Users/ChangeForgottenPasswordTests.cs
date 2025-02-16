using CareerCompass.Core.Common.Abstractions;
using CareerCompass.Core.Common.Abstractions.Crypto;
using CareerCompass.Core.Common.Abstractions.Repositories;
using CareerCompass.Core.Users.Commands.ChangeForgottenPassword;
using CareerCompass.Core.Users.Commands.GenerateForgotPasswordCode;
using CareerCompass.Tests.Fakers.Core;
using NSubstitute;

namespace CareerCompass.Tests.Unit.Core.Users;

public class ChangeForgottenPasswordTests
{
    private readonly UserFaker _userFaker = new();
    private readonly ICryptoService _cryptoService = Substitute.For<ICryptoService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ILoggerAdapter<ChangeForgottenPasswordCommandHandler> _logger =
        Substitute.For<ILoggerAdapter<ChangeForgottenPasswordCommandHandler>>();

    private readonly ChangeForgottenPasswordCommandHandler _sut;

    public ChangeForgottenPasswordTests()
    {
        _sut = new(_userRepository, _cryptoService, _logger);
    }

    [Fact]
    public async Task ShouldChangePassword_WhenInputIsValid()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public async Task ShouldReturnChangePasswordFailed_WhenDbFails()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public async Task ShouldReturnInvalidCodeError_WhenCodeIsInvalid()
    {
        // Arrange

        // Act

        // Assert
    }
}
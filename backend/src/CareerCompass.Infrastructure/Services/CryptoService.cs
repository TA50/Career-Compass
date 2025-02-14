using CareerCompass.Core.Common.Abstractions.Crypto;


namespace CareerCompass.Infrastructure.Services;

public class CryptoService : ICryptoService
{
    // private readonly CryptoSettings _settings = options.Value;

    public string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }

    public bool Verify(string input, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(input, hash);
    }

    public string Encrypt(string input)
    {
        throw new NotImplementedException();
    }

    public string Decrypt(string input)
    {
        throw new NotImplementedException();
    }
}
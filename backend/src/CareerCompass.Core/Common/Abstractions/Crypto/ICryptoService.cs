namespace CareerCompass.Core.Common.Abstractions.Crypto;

public interface ICryptoService
{
    string Hash(string input);

    bool Verify(string input, string hash);

    string Encrypt(string input);

    string Decrypt(string input);
}
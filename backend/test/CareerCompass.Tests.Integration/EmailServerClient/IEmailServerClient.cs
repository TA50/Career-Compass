namespace CareerCompass.Tests.Integration.EmailServerClient;

public interface IEmailServerClient
{
    public Task<List<IMessage>> GetMessages();

    public Task<string> GetRawMessage(string id);
}
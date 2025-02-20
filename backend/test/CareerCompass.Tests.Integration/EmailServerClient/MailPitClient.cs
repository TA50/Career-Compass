using System.Net.Http.Json;

namespace CareerCompass.Tests.Integration.EmailServerClient;

public class MailPitClient : IEmailServerClient
{
    private readonly HttpClient _httpClient;

    public MailPitClient(int port)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://localhost:{port}/api/v1/")
        };
    }


    public async Task<List<IMessage>> GetMessages()
    {
        var response = await _httpClient.GetFromJsonAsync<ListMessageResponse>("messages");

        if (response is null)
        {
            throw new Exception("Unable to get messages");
        }

        return response.Messages.Select(message =>
        {
            return new IMessage(message.Id, message.From.Address, message.To.Select(m => m.Address).ToList(),
                message.Subject);
        }).ToList();
    }

    public async Task<string> GetRawMessage(string id)
    {
        var response = await _httpClient.GetAsync($"message/{id}/raw");


        return await response.Content.ReadAsStringAsync();
    }
}
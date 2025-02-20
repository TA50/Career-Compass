namespace CareerCompass.Tests.Integration.EmailServerClient;

public record IMessage(string Id, string From, IList<string> To, string Subject);
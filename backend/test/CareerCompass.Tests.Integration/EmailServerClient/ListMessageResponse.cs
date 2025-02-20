using System.Text.Json.Serialization;

namespace CareerCompass.Tests.Integration.EmailServerClient;

public record ListMessageResponse(
    [property: JsonPropertyName("messages")]
    List<EmailMessage> Messages,
    [property: JsonPropertyName("messages_count")]
    int MessagesCount,
    [property: JsonPropertyName("start")] int Start,
    [property: JsonPropertyName("tags")] List<string> Tags,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("unread")] int Unread
);

public record EmailMessage(
    [property: JsonPropertyName("body")] MessageSummary Body,
    [property: JsonPropertyName("attachments")]
    int Attachments,
    [property: JsonPropertyName("bcc")] List<EmailAddress> Bcc,
    [property: JsonPropertyName("cc")] List<EmailAddress> Cc,
    [property: JsonPropertyName("created")]
    DateTime Created,
    [property: JsonPropertyName("from")] EmailAddress From,
    [property: JsonPropertyName("ID")] string Id,
    [property: JsonPropertyName("MessageID")]
    string MessageId,
    [property: JsonPropertyName("read")] bool Read,
    [property: JsonPropertyName("replyTo")]
    List<EmailAddress> ReplyTo,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("snippet")]
    string Snippet,
    [property: JsonPropertyName("subject")]
    string Subject,
    [property: JsonPropertyName("tags")] List<string> Tags,
    [property: JsonPropertyName("to")] List<EmailAddress> To
);

public record MessageSummary(
    [property: JsonPropertyName("summary")]
    string Summary // Modify based on actual structure
);

public record EmailAddress(
    [property: JsonPropertyName("address")]
    string Address,
    [property: JsonPropertyName("name")] string Name
);
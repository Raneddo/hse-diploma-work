namespace ADSD.Backend.App.Models;

public class ChatMessage
{
    public ChatMessage(long id, long chatId, int fromId, string fromName, string message, DateTimeOffset createdAt)
    {
        Id = id;
        ChatId = chatId;
        FromId = fromId;
        FromName = fromName;
        Message = message;
        CreatedAt = createdAt;
    }

    public long Id { get; }
    public long ChatId { get; }
    public int FromId { get; }
    public string FromName { get; }
    public string Message { get; }
    public DateTimeOffset CreatedAt { get; }
}
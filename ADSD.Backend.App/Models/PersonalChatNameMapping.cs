namespace ADSD.Backend.App.Models;

public class PersonalChatNameMapping
{
    public PersonalChatNameMapping(long chatId, string name)
    {
        ChatId = chatId;
        Name = name;
    }

    public long ChatId { get; }
    public string Name { get; }
}
namespace ADSD.Backend.App.Models;

public class ChatShortInfo
{
    public ChatShortInfo(long id, ChatTypeEnum type, string name = null)
    {
        Id = id;
        Type = type;

    }

    public long Id { get; }
    public ChatTypeEnum Type { get; }
    public string Name { get; set; }
}

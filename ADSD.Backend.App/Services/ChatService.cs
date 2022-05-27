using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Exceptions;
using ADSD.Backend.App.Models;

namespace ADSD.Backend.App.Services;

public class ChatService
{
    private readonly AppDbClient _appDbClient;
    private readonly NotificationsService _notificationsService;

    public ChatService(AppDbClient appDbClient, NotificationsService notificationsService)
    {
        _appDbClient = appDbClient;
        _notificationsService = notificationsService;
    }

    public IEnumerable<ChatShortInfo> GetChatsByUser(int userId)
    {
        var chats =  _appDbClient.GetChatsByUser(userId).ToList();
        var secondUserNameByChatId = _appDbClient.GetSecondUserNameByUserId(userId)
            .ToDictionary(x => x.ChatId, x => x.Name);
        
        foreach (var chat in chats.Where(chat => chat.Type == ChatTypeEnum.Personal))
        {
            chat.Name = secondUserNameByChatId[chat.Id];
        }

        return chats;
    }

    public long CreatePersonalChat(int firstUserId, int secondUserId)
    {
        var personalChatId = _appDbClient.GetPersonalChatId(firstUserId, secondUserId);
        if (personalChatId.HasValue)
        {
            return personalChatId.Value;
        }
        
        var chatId = _appDbClient.CreateChat("Personal", ChatTypeEnum.Personal);
        _appDbClient.LinkPersonalChatToUsers(chatId, firstUserId, secondUserId);
        _appDbClient.AddUsersToChat(chatId, new[] {firstUserId, secondUserId});

        return chatId;
    }
    
    public long CreateGroupChat(string name, int userId, int[] users)
    {
        var chatId = _appDbClient.CreateChat(name, ChatTypeEnum.Group);
        _appDbClient.AddUsersToChat(chatId, new []{userId});
        if (users.Length > 0)
        {
            _appDbClient.AddUsersToChat(chatId, users);
        }

        return chatId;
    }

    public long CreateChannel(string name)
    {
        var chatId = _appDbClient.CreateChat(name, ChatTypeEnum.Channel);

        return chatId;
    }

    public long SendMessage(bool isAdmin, int userId, long chatId, string message)
    {
        var ok = _appDbClient.UserInChat(chatId, userId);
        if (!ok)
        {
            throw new ArgumentOutOfRangeException();
        }

        var chat = _appDbClient.ChatById(chatId);
        if (chat == null)
        {
            throw new ArgumentNullException();
        }

        if (chat.Type == ChatTypeEnum.Channel && !isAdmin)
        {
            throw new ArgumentOutOfRangeException();
        }

        var messageId = _appDbClient.SendMessageToChat(userId, chatId, message);

        var chatUsers = _appDbClient.GetChatUsers(chatId).ToList();
        _notificationsService.SendPush(chatUsers.Except(new []{userId}), message);

        return messageId;
    }

    public IEnumerable<ChatMessage> GetChatMessages(long chatId, int userId, long afterMessageId = 0, long count = int.MaxValue)
    {
        if (!_appDbClient.UserInChat(chatId, userId))
        {
            throw new NotFoundException("You have no access to this chat or chat doesn't exists");
        }

        var chatMessages = _appDbClient.GetChatMessages(chatId, afterMessageId, count);
        return chatMessages;
    }
}
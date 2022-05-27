namespace ADSD.Backend.App.Services;

public class NotificationsService
{
    public void SendPush(IEnumerable<int> users, string message)
    {
        foreach (var user in users)
        {
            Console.WriteLine($"Send {message} to {user}");
        }
    }
}
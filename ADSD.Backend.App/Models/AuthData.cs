namespace ADSD.Backend.App.Models;

public class AuthData
{
    public AuthData(string userName, string[] groups)
    {
        UserName = userName;
        Groups = groups;
    }

    public string UserName { get; }
    public string[] Groups { get; }
}
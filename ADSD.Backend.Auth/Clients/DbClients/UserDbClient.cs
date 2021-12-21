using System;
using ADSD.Backend.Auth.Models;

namespace ADSD.Backend.Auth.Clients.DbClients;

public class UserDbClient
{
    public UserInfo GetUserInfo(int userId)
    {
        throw new NotImplementedException();
        return new UserInfo()
        {
            Email = "email@email.email",
        };
    }
}
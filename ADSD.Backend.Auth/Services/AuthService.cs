using System;
using ADSD.Backend.Auth.Clients.DbClients;
using ADSD.Backend.Auth.Models;

namespace ADSD.Backend.Auth.Services
{
    public class AuthService
    {
        private readonly LinkDbClient _linkDbClient;

        public AuthService(LinkDbClient linkDbClient)
        {
            _linkDbClient = linkDbClient;
        }
        
        public UserInfo RegisterUserByLink(string link, UserInfo userInfo)
        {
            var userId = _linkDbClient.GetUserIdByLink(link);
            if (!userId.HasValue)
            {
                throw new Exception("User not found");
            }

            throw new NotImplementedException();
        }
    }
}
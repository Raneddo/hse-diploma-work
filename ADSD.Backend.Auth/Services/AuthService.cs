using System;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using ADSD.Backend.Auth.Clients.DbClients;
using ADSD.Backend.Auth.Models;

namespace ADSD.Backend.Auth.Services
{
    public class AuthService
    {
        private readonly LinkDbClient _linkDbClient;
        private readonly UserCredentialsDbClient _userCredentialsDbClient;

        public AuthService(LinkDbClient linkDbClient, UserCredentialsDbClient userCredentialsDbClient)
        {
            _linkDbClient = linkDbClient;
            _userCredentialsDbClient = userCredentialsDbClient;
        }
        
        public Task<UserInfo> RegisterUserByLink(string link, UserInfo userInfo)
        {
            var userId = _linkDbClient.GetUserIdByLink(link);
            if (!userId.HasValue)
            {
                throw new Exception("User not found");
            }

            throw new NotImplementedException();
        }

        public async Task<string> Login(string userName, string password)
        {
            var passHash = Helpers.SecureHelper.GenerateSecuredPassword(password, Encoding.Default.GetBytes("ADSD"));
            var userId = await _userCredentialsDbClient.GetUserIdByCredentials(userName, passHash);

            if (!userId.HasValue)
                throw new AuthenticationException("Invalid username or password");
            
            var basicToken = Helpers.SecureHelper.GenerateSecuredBasicToken(userName, passHash);
            
            await _userCredentialsDbClient.AddUserToken(userId.Value, $"Basic {basicToken}");
            
            return basicToken;
        }
    }
}
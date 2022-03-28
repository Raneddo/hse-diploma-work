using System.Text;
using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Helpers;
using ADSD.Backend.App.Models;

namespace ADSD.Backend.App.Services
{
    public class AuthService
    {
        private readonly AppDbClient _appDbClient;

        public AuthService(AppDbClient appDbClient)
        {
            _appDbClient = appDbClient;
        }

        public int Login(string userName, string password)
        {
            var passHash = SecureHelper.GenerateSecuredPassword(password, Encoding.Default.GetBytes("ADSD"));
            var userId = _appDbClient.GetUserIdByCredentials(userName, passHash);

            
            return userId;
        }

        public int Register(string userName, string passHash)
        {
            return _appDbClient.RegisterUser(userName, passHash);
        }

        public void ActivateAccount(int userId)
        {
            _appDbClient.AuthActivate(userId);
        }

        public int HardLogin(string email)
        {
            return _appDbClient.FindUserByEmail(email);
        }
        
        public void ChangeCredentials(int userId, string userName, string passHash)
        {
            _appDbClient.AuthUpdate(userId, userName, passHash);
        }

        public AuthUser GetUser(int userId)
        {
            return _appDbClient.GetAuthUser(userId);
        }
    }
}
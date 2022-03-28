using System.Text;
using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Helpers;

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
    }
}
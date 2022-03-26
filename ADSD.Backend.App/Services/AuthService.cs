using System.Text;
using ADSD.Backend.App.Clients;

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
            var passHash = Helpers.SecureHelper.GenerateSecuredPassword(password, Encoding.Default.GetBytes("ADSD"));
            var userId = _appDbClient.GetUserIdByCredentials(userName, passHash);

            
            return userId;
        }
    }
}
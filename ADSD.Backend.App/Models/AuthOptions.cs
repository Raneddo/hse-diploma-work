using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ADSD.Backend.App.Models
{
    public class AuthOptions
    {
        public const string Issuer = "ADSD.Issuer"; // издатель токена
        public const string Audience = "ADSD.ClientApp"; // потребитель токена
        private static string Key => Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new Exception("Has no SECRET_KEY env var");   // ключ для шифрации
        public const int Lifetime = 3600; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
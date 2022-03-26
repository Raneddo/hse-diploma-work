using System.Security.Cryptography;
using System.Text;

namespace ADSD.Backend.App.Helpers;

public static class SecureHelper
{
    public static string GenerateSecuredPassword(string password, IEnumerable<byte> salt)
    {
        var hasher = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password)
            .Concat(salt)
            .ToArray();
        return BitConverter.ToString(hasher.ComputeHash(bytes)).Replace("-", string.Empty);
    }

    public static string GenerateSecuredBasicToken(string userName, string passHash)
    {
        var bytes = Encoding.UTF8.GetBytes($"{userName}:{passHash}");

        var base64String = Convert.ToBase64String(bytes);
        return base64String;
    }
}
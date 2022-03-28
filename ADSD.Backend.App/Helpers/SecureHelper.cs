using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ADSD.Backend.App.Models;
using static System.Enum;

namespace ADSD.Backend.App.Helpers;

public static class SecureHelper
{
    private static readonly string AllowedPasswordSymbols =
        "QWERTYUOPASDFGHJKZXCVBNMqwertyuopasdfghjkzxcvbnm1234567890";

    private static readonly Random Random = new Random(DateTime.Now.Millisecond);

    public static string GenerateRandomPassword()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < 8; i++)
        {
            sb.Append(AllowedPasswordSymbols[Random.Next(0, AllowedPasswordSymbols.Length)]);
        }

        return sb.ToString();
    }
    
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

    public static List<UserRole> GetUserRoles(this ClaimsPrincipal currentUser)
    {
        if (!currentUser.HasClaim(x => x.Type == ClaimsIdentity.DefaultRoleClaimType))
        {
            return new List<UserRole>();
        }
        
        var roles = currentUser.Claims
            .Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)
            .Select(x =>
            {
                _ = TryParse<UserRole>(x.Value, out var role);
                return role;
            })
            .ToList();
        return roles;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ADSD.Backend.Auth.Helpers;

public static class SecureHelper
{
    public static string GenerateSecuredPassword(string password, IEnumerable<byte> salt)
    {
        var hasher = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(password)
            .Concat(salt)
            .ToArray();
        return BitConverter.ToString(hasher.ComputeHash(bytes));
    }
}
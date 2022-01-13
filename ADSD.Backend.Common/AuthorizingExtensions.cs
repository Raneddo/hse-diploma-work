namespace ADSD.Backend.Common;

using Microsoft.AspNetCore.Http;

public static class AuthorizingExtensions
{
    public static string? GetBasicToken(this IHeaderDictionary headerDictionary)
    {
        return headerDictionary.Authorization.FirstOrDefault() == "Basic"
            ? headerDictionary.Authorization.Last()
            : null;
    }
}
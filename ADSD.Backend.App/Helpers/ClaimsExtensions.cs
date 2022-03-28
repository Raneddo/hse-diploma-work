using System.Security.Claims;

namespace ADSD.Backend.App.Helpers;

public static class ClaimsExtensions
{
    public static int? GetUserId(this ClaimsPrincipal currentUser)
    {
        var sid = currentUser.FindFirst("id")?.Value;
        if (int.TryParse(sid, out var userId))
        {
            return userId;
        }

        return null;
    }
}
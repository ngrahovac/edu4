using System.Security.Claims;

namespace edu4.API.Utils;

public static class AuthorizationUtils
{
    public static string ExtractAccountId(HttpRequest request)
    {
        var claimsIdentity = request.HttpContext.User.Identity as ClaimsIdentity;
        var accountId = claimsIdentity?.Name;

        if (claimsIdentity is null || accountId is null)
        {
            throw new ArgumentException("Could not extract account id from bearer token");
        }

        return accountId;
    }
}

using System.Security.Claims;

namespace Peer.API.Utils;

public class AccountIdExtractionService : IAccountIdExtractionService
{
    public string ExtractAccountIdFromHttpRequest(HttpRequest request)
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
